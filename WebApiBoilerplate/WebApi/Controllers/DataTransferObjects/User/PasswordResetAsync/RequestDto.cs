namespace WebApi.Controllers.DataTransferObjects.User.PasswordResetAsync
{
    /// <summary>
    /// Data transfer object for the "PasswordReset" request.
    /// </summary>
    public class RequestDto
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }
    }
}