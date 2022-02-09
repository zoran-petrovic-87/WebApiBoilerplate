using System;
using System.Globalization;

namespace WebApi.Helpers.Exceptions;

/// <summary>
/// Exception that should be thrown when changing read-only properties.
/// </summary>
/// <seealso cref="WebApi.Helpers.Exceptions.AppException" />
public class UpdateReadOnlyPropertyException : AppException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateReadOnlyPropertyException"/> class.
    /// </summary>
    public UpdateReadOnlyPropertyException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateReadOnlyPropertyException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UpdateReadOnlyPropertyException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateReadOnlyPropertyException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    public UpdateReadOnlyPropertyException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}