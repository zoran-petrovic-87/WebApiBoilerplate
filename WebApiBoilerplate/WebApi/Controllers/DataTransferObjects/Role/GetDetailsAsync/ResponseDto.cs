using System;

namespace WebApi.Controllers.DataTransferObjects.Role.GetDetailsAsync
{
    /// <summary>
    /// Data transfer object for the role "GetDetails" response.
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
        /// Gets or sets the date when the role was created.
        /// </summary>
        /// <value>
        /// The date when the role was created.
        /// </value>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user that created this role.
        /// </summary>
        /// <value>
        /// The identifier of the user that created this role.
        /// </value>
        public Guid CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the date when the role was updated.
        /// </summary>
        /// <value>
        /// The date when the role was updated.
        /// </value>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user that updated this role.
        /// </summary>
        /// <value>
        /// The identifier of the user that updated this role.
        /// </value>
        public Guid UpdatedById { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }
    }
}