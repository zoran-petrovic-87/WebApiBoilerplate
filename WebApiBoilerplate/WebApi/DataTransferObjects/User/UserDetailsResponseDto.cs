using System;

// ReSharper disable MemberCanBePrivate.Global

namespace WebApi.DataTransferObjects.User
{
    /// <summary>
    /// Data transfer object for the user details response.
    /// </summary>
    public class UserDetailsResponseDto
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }

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
        /// Gets or sets the role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the date when this user was created.
        /// </summary>
        /// <value>
        /// The date when this user was created.
        /// </value>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date when this user was updated.
        /// </summary>
        /// <value>
        /// The date when this user was updated.
        /// </value>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date of the last successful login.
        /// </summary>
        /// <value>
        /// The date of the last successful login.
        /// </value>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this user is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDetailsResponseDto"/> class.
        /// </summary>
        public UserDetailsResponseDto()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDetailsResponseDto"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        public UserDetailsResponseDto(Models.User user)
        {
            Id = user.Id;
            Username = user.Username;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            CreatedAt = user.CreatedAt;
            UpdatedAt = user.UpdatedAt;
            LastLoginAt = user.LastLoginAt;
            IsActive = user.IsActive;
            Role = user.Role;
        }
    }
}