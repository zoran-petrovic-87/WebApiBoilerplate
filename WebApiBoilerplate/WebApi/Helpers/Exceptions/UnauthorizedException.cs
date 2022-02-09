using System;
using System.Globalization;

namespace WebApi.Helpers.Exceptions;

/// <summary>
/// Exception that should be thrown when user is unauthorized.
/// </summary>
/// <seealso cref="WebApi.Helpers.Exceptions.AppException" />
public class UnauthorizedException : AppException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
    /// </summary>
    public UnauthorizedException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UnauthorizedException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    public UnauthorizedException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}