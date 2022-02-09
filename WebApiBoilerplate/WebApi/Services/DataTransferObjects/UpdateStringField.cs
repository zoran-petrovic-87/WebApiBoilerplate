namespace WebApi.Services.DataTransferObjects;

/// <summary>
/// Class used in data transfer objects for updating string fields.
/// If the object is <c>null</c> then the field shouldn't be updated.
/// If the object is not <c>null</c> then the field should be set to the <c>NewValue</c> property of the object.
/// </summary>
public class UpdateStringField
{
    /// <summary>
    /// Gets or sets the new value.
    /// </summary>
    /// <value>
    /// The new value.
    /// </value>
    public string NewValue { get; set; }
}