using System;
using System.Globalization;

namespace WebApi.Helpers.Exceptions;

/// <summary>
/// Exception that should be thrown when there were too many failed attempts to reset password.
/// </summary>
/// <seealso cref="WebApi.Helpers.Exceptions.AppException" />
public class TooManyResetPasswordAttemptsException : AppException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TooManyResetPasswordAttemptsException"/> class.
    /// </summary>
    public TooManyResetPasswordAttemptsException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TooManyResetPasswordAttemptsException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TooManyResetPasswordAttemptsException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TooManyResetPasswordAttemptsException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    public TooManyResetPasswordAttemptsException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}