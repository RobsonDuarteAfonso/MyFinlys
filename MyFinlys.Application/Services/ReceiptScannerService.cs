using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;

namespace MyFinlys.Application.Services;

public class ReceiptScannerService : IReceiptScannerService
{
    private readonly IConfiguration _configuration;

    public ReceiptScannerService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<ReceiptScanResultDto?> ScanReceiptAsync(byte[] imageBytes, string contentType)
    {
        var apiKey = _configuration["GeminiSettings:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Gemini API Key is not configured. Please add 'GeminiSettings:ApiKey' to your appsettings.json.");
        }

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.5-flash:generateContent?key={apiKey}";
        var base64Image = Convert.ToBase64String(imageBytes);

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {
                        new {
                            text = "Analyze this receipt or invoice image. Extract the following information:\n" +
                                   "1. \"Value\" - the total value of the transaction as a decimal number (e.g. 45.90). Parse BRL (R$), CAD$, or USD format.\n" +
                                   "2. \"Description\" - a short description or merchant name (e.g. \"Supermercado Extra\", \"Uber\").\n" +
                                   "3. \"Due\" - the date of the purchase in YYYY-MM-DD format. To parse the date correctly, detect the language/locale of the receipt:\n" +
                                   "   - If the receipt is in French (e.g., Canada/Quebec), the date format is usually YY/MM/DD or YYYY/MM/DD (Year/Month/Day). For example, '26/07/16' means 2026-07-16.\n" +
                                   "   - If the receipt is in English, the format is usually MM/DD/YY or MM/DD/YYYY (Month/Day/Year).\n" +
                                   "   - If the receipt is in Portuguese, the format is usually DD/MM/YY or DD/MM/YYYY (Day/Month/Year).\n" +
                                   "   - For 2-digit years, expand them to the 2000s (e.g., '26' -> '2026'). If the date is missing, assume the current year 2026.\n" +
                                   "4. \"Category\" - choose one of the following exact categories that best fits the purchase: Salary, Groceries, Restaurant, Vehicle, Transportation, Housing, Leisure, Pharmacy, Health, Education, Government, Transfer, Travel, Shopping, CreditCard, Telephony, Tax, Others.\n\n" +
                                   "Return ONLY a JSON object with properties: \"Value\", \"Description\", \"Due\", \"Category\"."
                        },
                        new {
                            inlineData = new
                            {
                                mimeType = contentType,
                                data = base64Image
                            }
                        }
                    }
                }
            },
            generationConfig = new
            {
                responseMimeType = "application/json"
            }
        };

        using var client = new HttpClient();
        var jsonContent = JsonSerializer.Serialize(requestBody);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, httpContent);
        if (!response.IsSuccessStatusCode)
        {
            var errorMsg = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Gemini API error ({response.StatusCode}): {errorMsg}");
        }

        var responseString = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseString);
        
        // Gemini API response structure: candidates[0].content.parts[0].text
        var root = doc.RootElement;
        if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
        {
            var candidate = candidates[0];
            if (candidate.TryGetProperty("content", out var content) &&
                content.TryGetProperty("parts", out var parts) && parts.GetArrayLength() > 0)
            {
                var textPart = parts[0].GetProperty("text").GetString();
                if (!string.IsNullOrWhiteSpace(textPart))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return JsonSerializer.Deserialize<ReceiptScanResultDto>(textPart, options);
                }
            }
        }

        return null;
    }
}
