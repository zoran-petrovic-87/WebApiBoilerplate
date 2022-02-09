using System.Threading.Tasks;

namespace WebApi.Services.Interfaces;

/// <summary>
/// The email service interface.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends the email asynchronously.
    /// </summary>
    /// <param name="fromEmail">The sender email address.</param>
    /// <param name="fromName">The sender name.</param>
    /// <param name="toEmail">The recipient email address.</param>
    /// <param name="subject">The email subject.</param>
    /// <param name="htmlContent">The HTML content.</param>
    /// <param name="plainTextContent">The plain text content.</param>
    /// <returns><c>true</c> if sending was successful, <c>false</c> otherwise.</returns>
    Task<bool> SendAsync(string fromEmail, string fromName, string toEmail, string subject, string htmlContent,
        string plainTextContent);
}