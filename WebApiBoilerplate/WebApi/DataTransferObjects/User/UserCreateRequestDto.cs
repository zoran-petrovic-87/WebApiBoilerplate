using System;

namespace WebApi.DataTransferObjects.User
{
    /// <summary>
    /// Data transfer object for the create user request.
    /// </summary>
    public class UserCreateRequestDto
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Creates the user from available data in this object.
        /// </summary>
        /// <returns>The user.</returns>
        public Models.User CreateUser()
        {
            return new Models.User
            {
                Id = Guid.NewGuid(),
                FirstName = FirstName,
                LastName = LastName,
                Username = Username,
                Email = Email
            };
        }
    }
}