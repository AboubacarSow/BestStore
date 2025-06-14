﻿using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BestStoreApp.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var apiKey = _configuration.GetSection("SendGridSettings:BestStore").Value;
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("Null SendGridKey");
        }
        await Execute(apiKey, subject, htmlMessage, email);
    }

    public async Task Execute(string apiKey, string subject, string message, string toEmail)
    {
        var client = new SendGridClient(apiKey);
        var senderEmail = _configuration.GetSection("SendGridSettings:SenderEmail").Value;
        var senderName = _configuration.GetSection("SendGridSettings:SenderName").Value ?? "Reset Password";
        var msg = new SendGridMessage()
        {
            From = new EmailAddress(senderEmail, senderName),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
        };
        msg.AddTo(new EmailAddress(toEmail));

        // Disable click tracking.
        msg.SetClickTracking(false, false);
        var response = await client.SendEmailAsync(msg);
        _logger.LogInformation(!response.IsSuccessStatusCode
                               ? $"Failure Email to {toEmail}"
                               : $"Email to {toEmail} queued successfully!");
    }
}
