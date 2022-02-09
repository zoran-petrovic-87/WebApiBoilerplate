using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using WebApi.Helpers;
using WebApi.Services.Interfaces;

namespace WebApi.Services;

/// <summary>
/// The email service.
/// </summary>
/// <seealso cref="IEmailService" />
public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly AppSettings _appSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="appSettings">The application settings.</param>
    public EmailService(ILogger<EmailService> logger, IOptions<AppSettings> appSettings)
    {
        _logger = logger;
        _appSettings = appSettings.Value;
    }

    /// <inheritdoc />
    public async Task<bool> SendAsync(string fromEmail, string fromName, string toEmail, string subject,
        string htmlContent, string plainTextContent)
    {
        // TODO: use dependency injection when SendGrid starts supporting it.
        var client = new SendGridClient(_appSettings.SendGridApiKey);
        var from = new EmailAddress(fromEmail, fromName);
        var to = new EmailAddress(toEmail);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);

        if (response.StatusCode != HttpStatusCode.Accepted)
        {
            var responseBody = await response.Body.ReadAsStringAsync();
            _logger.LogError($"Error sending email to {toEmail} with subject '{subject}'.\n " +
                             $"Status code: {response.StatusCode}.\n " +
                             $"Response body: {responseBody}");
            return false;
        }

        return true;
    }
}