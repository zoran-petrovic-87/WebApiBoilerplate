using System;
using System.Globalization;

namespace WebApi.Helpers.Exceptions;

/// <summary>
/// Exception that should be thrown when the email is already taken.
/// </summary>
/// <seealso cref="WebApi.Helpers.Exceptions.AppException" />
public class EmailTakenException : AppException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailTakenException"/> class.
    /// </summary>
    public EmailTakenException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailTakenException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public EmailTakenException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailTakenException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    public EmailTakenException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}