namespace WebApi.Controllers.DataTransferObjects.User.PasswordReset
{
    /// <summary>
    /// Data transfer object for the password reset request.
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