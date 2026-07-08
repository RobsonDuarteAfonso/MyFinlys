using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MyFinlys.Application.Services.Interfaces;

namespace MyFinlys.Infrastructure.Services;

public class EmailSettings
{
    public string SmtpHost { get; set; } = "smtp.gmail.com";
    public int SmtpPort { get; set; } = 587;
    public string SmtpUser { get; set; } = string.Empty;
    public string SmtpPass { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = "MyFinlys";
}

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string toName, string resetLink)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = "Reset your MyFinlys password";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $$"""
                <!DOCTYPE html>
                <html>
                <head>
                  <meta charset="utf-8" />
                  <style>
                    body { font-family: 'Segoe UI', Arial, sans-serif; background: #f8fafc; margin: 0; padding: 0; }
                    .container { max-width: 520px; margin: 40px auto; background: #fff; border-radius: 16px; border: 1px solid #e2e8f0; overflow: hidden; }
                    .header { background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%); padding: 32px 40px; text-align: center; }
                    .header h1 { color: white; margin: 0; font-size: 24px; font-weight: 700; }
                    .header p { color: #bfdbfe; margin: 4px 0 0; font-size: 13px; }
                    .body { padding: 36px 40px; }
                    .body p { color: #475569; font-size: 15px; line-height: 1.7; margin: 0 0 16px; }
                    .btn { display: inline-block; background: linear-gradient(135deg, #2563eb, #1d4ed8); color: #fff !important; text-decoration: none; padding: 14px 32px; border-radius: 10px; font-size: 15px; font-weight: 600; margin: 8px 0 20px; }
                    .notice { background: #f8fafc; border-radius: 8px; padding: 14px 16px; font-size: 13px; color: #94a3b8; margin-top: 24px; }
                    .footer { padding: 16px 40px; text-align: center; font-size: 12px; color: #cbd5e1; border-top: 1px solid #f1f5f9; }
                  </style>
                </head>
                <body>
                  <div class="container">
                    <div class="header">
                      <h1>MyFinlys</h1>
                      <p>Smart Finance Platform</p>
                    </div>
                    <div class="body">
                      <p>Hi <strong>{{toName}}</strong>,</p>
                      <p>We received a request to reset your password. Click the button below to create a new one. This link expires in <strong>1 hour</strong>.</p>
                      <div style="text-align:center">
                        <a href="{{resetLink}}" class="btn">Reset My Password</a>
                      </div>
                      <div class="notice">
                        If you didn't request a password reset, you can safely ignore this email. Your password will remain unchanged.
                      </div>
                    </div>
                    <div class="footer">© {{DateTime.UtcNow.Year}} MyFinlys. All rights reserved.</div>
                  </div>
                </body>
                </html>
                """
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
