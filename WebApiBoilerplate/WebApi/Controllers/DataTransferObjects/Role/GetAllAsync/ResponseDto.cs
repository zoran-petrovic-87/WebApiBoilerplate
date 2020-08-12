using System;

namespace WebApi.Controllers.DataTransferObjects.Role.GetAllAsync
{
    /// <summary>
    /// Data transfer object for the role summary response.
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
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
}