using System;
using System.Globalization;

namespace WebApi.Helpers.Exceptions;

/// <summary>
/// Exception that should be thrown when password has invalid format.
/// </summary>
/// <seealso cref="WebApi.Helpers.Exceptions.AppException" />
public class InvalidPasswordException : AppException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPasswordException"/> class.
    /// </summary>
    public InvalidPasswordException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPasswordException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidPasswordException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPasswordException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    public InvalidPasswordException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}