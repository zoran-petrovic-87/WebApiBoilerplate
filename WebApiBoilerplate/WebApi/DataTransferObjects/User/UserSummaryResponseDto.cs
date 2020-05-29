using System;

// ReSharper disable MemberCanBePrivate.Global

namespace WebApi.DataTransferObjects.User
{
    /// <summary>
    /// Data transfer object for the user summary response.
    /// </summary>
    public class UserSummaryResponseDto
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }

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
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        public string Role { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSummaryResponseDto"/> class.
        /// </summary>
        public UserSummaryResponseDto()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserSummaryResponseDto"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        public UserSummaryResponseDto(Models.User user)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Username;
            Email = user.Email;
            Role = user.Role;
        }
    }
}