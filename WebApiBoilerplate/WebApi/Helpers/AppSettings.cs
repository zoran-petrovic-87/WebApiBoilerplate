namespace WebApi.Helpers;

/// <summary>
/// Class used to map strongly typed settings objects.
/// </summary>
public class AppSettings
{
    /// <summary>
    /// Gets or sets the secret used for encryption. Can be any text that is at least 16 characters long.
    /// </summary>
    /// <value>
    /// The secret used for encryption.
    /// </value>
    public string Secret { get; set; }

    /// <summary>
    /// Gets or sets the SendGrid API key.
    /// </summary>
    /// <value>
    /// The SendGrid API key.
    /// </value>
    public string SendGridApiKey { get; set; }

    /// <summary>
    /// Gets or sets the name of the WebAPI.
    /// </summary>
    /// <value>
    /// The name of the WebAPI.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the email address that will be used to send emails.
    /// </summary>
    /// <value>
    /// The email address that will be used to send emails.
    /// </value>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the name of the email sender.
    /// </summary>
    /// <value>
    /// The name of the email sender.
    /// </value>
    public string EmailName { get; set; }

    /// <summary>
    /// Gets or sets the maximum count of the failed login attempts.
    /// </summary>
    /// <value>
    /// The maximum count of the failed login attempts.
    /// </value>
    public int MaxLoginFailedCount { get; set; }

    /// <summary>
    /// Gets or sets the time (in seconds) that the user has to wait if they exceeded the maximum count of
    /// the failed login attempts.
    /// </summary>
    /// <value>
    /// The login failed waiting time (in seconds) that the user has to wait if they exceeded the maximum count of
    /// the failed login attempts.
    /// </value>
    public int LoginFailedWaitingTime { get; set; }

    /// <summary>
    /// Gets or sets the maximum count of the unconfirmed email addresses.
    /// </summary>
    /// <value>
    /// The maximum count of the unconfirmed email addresses.
    /// </value>
    public int MaxUnconfirmedEmailCount { get; set; }

    /// <summary>
    /// Gets or sets the time (in seconds) that the user has to wait if they exceeded the maximum count of
    /// the unconfirmed email addresses, before they try to change their email address again.
    /// </summary>
    /// <value>
    /// The time (in seconds) that the user has to wait if they exceeded the maximum count of
    /// the unconfirmed email addresses, before they try to change their email address again.
    /// </value>
    public int UnconfirmedEmailWaitingTime { get; set; }

    /// <summary>
    /// Gets or sets the URL that will be used to confirm email address.
    /// </summary>
    /// <value>
    /// The URL that will be used to confirm email address.
    /// </value>
    public string ConfirmEmailUrl { get; set; }

    /// <summary>
    /// Gets or sets the maximum count of the failed reset password attempts.
    /// </summary>
    /// <value>
    /// The maximum count of the failed reset password attempts.
    /// </value>
    public int MaxResetPasswordCount { get; set; }

    /// <summary>
    /// Gets or sets the time (in seconds) that the user has to wait if they exceeded the maximum count of
    /// the failed reset password attempts.
    /// </summary>
    /// <value>
    /// The time (in seconds) that the user has to wait if they exceeded the maximum count of
    /// the failed reset password attempts.
    /// </value>
    public int ResetPasswordWaitingTime { get; set; }

    /// <summary>
    /// Gets or sets the validity time for the reset password request (in seconds).
    /// </summary>
    /// <value>
    /// The validity time for the reset password request (in seconds).
    /// </value>
    public int ResetPasswordValidTime { get; set; }

    /// <summary>
    /// Gets or sets the URL that will be used to confirm password reset.
    /// </summary>
    /// <value>
    /// The URL that will be used to confirm password reset.
    /// </value>
    public string ResetPasswordUrl { get; set; }

    /// <summary>
    /// Gets or sets the administrator username.
    /// </summary>
    /// <value>
    /// The administrator username.
    /// </value>
    public string AdminUsername { get; set; }

    /// <summary>
    /// Gets or sets the administrator email.
    /// </summary>
    /// <value>
    /// The administrator email.
    /// </value>
    public string AdminEmail { get; set; }

    /// <summary>
    /// Gets or sets the administrator password.
    /// </summary>
    /// <value>
    /// The administrator password.
    /// </value>
    public string AdminPassword { get; set; }

    /// <summary>
    /// Gets or sets Google client ID.
    /// </summary>
    public string OidcGoogleClientId { get; set; }
        
    /// <summary>
    /// Gets or sets Google client secret.
    /// </summary>
    public string OidcGoogleClientSecret { get; set; }
}