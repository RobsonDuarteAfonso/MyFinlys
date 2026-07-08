namespace MyFinlys.Application.DTOs;

public record ForgotPasswordDto(string Email);

public record ResetPasswordDto(string Token, string NewPassword);
