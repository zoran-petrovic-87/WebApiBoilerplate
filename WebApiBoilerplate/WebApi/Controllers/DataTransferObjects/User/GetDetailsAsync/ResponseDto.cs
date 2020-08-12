using System;

// ReSharper disable MemberCanBePrivate.Global

namespace WebApi.Controllers.DataTransferObjects.User.GetDetailsAsync
{
    /// <summary>
    /// Data transfer object for the user details response.
    /// </summary>
    public class ResponseDto
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
        public Role.GetAllAsync.ResponseDto Role { get; set; }

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
    }
}