using System;
using System.Globalization;

namespace WebApi.Helpers.Exceptions;

/// <summary>
/// Custom exception class for throwing application specific exceptions (e.g. for validation) 
/// that can be caught and handled within the application.
/// </summary>
/// <seealso cref="System.Exception" />
public class AppException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class.
    /// </summary>
    public AppException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AppException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AppException"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="args">The arguments.</param>
    public AppException(string message, params object[] args)
        : base(string.Format(CultureInfo.CurrentCulture, message, args))
    {
    }
}