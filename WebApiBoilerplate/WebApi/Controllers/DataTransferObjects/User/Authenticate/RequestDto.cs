namespace WebApi.Controllers.DataTransferObjects.User.Authenticate
{
    /// <summary>
    /// Data transfer object for the authenticate request.
    /// </summary>
    public class RequestDto
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }
    }
}