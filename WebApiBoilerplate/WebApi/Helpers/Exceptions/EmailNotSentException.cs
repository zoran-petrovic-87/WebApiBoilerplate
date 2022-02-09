using System;
using System.Globalization;

namespace WebApi.Helpers.Exceptions;

/// <summary>
/// Exception that should be thrown when the email is not sent successfully.
/// </summary>
/// <seealso cref="WebApi.Helpers.Exceptions.AppException" />
public class EmailNotSentException : AppException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailNotSentException"/> class.
    /// </summary>
    public EmailNotSentException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailNotSentException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public EmailNotSentException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailNotSentException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    public EmailNotSentException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}