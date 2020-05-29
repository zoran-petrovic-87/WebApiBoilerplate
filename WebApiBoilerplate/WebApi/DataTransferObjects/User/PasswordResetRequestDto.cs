using System;

namespace WebApi.DataTransferObjects.User
{
    /// <summary>
    /// Data transfer object for the password reset request.
    /// </summary>
    public class PasswordResetRequestDto
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