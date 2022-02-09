using System;

namespace WebApi.Models;

/// <summary>
/// Model used to store logs.
/// </summary>
public class Log
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>
    /// The identifier.
    /// </value>
    public long Id { get; set; }

    /// <summary>
    /// Gets or sets the application.
    /// </summary>
    /// <value>
    /// The application.
    /// </value>
    public string Application { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this log entry was created.
    /// </summary>
    /// <value>
    /// The date and time when this log entry was created.
    /// </value>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the level.
    /// </summary>
    /// <value>
    /// The level.
    /// </value>
    public string Level { get; set; }

    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    /// <value>
    /// The message.
    /// </value>
    public string Message { get; set; }

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    /// <value>
    /// The logger.
    /// </value>
    public string Logger { get; set; }

    /// <summary>
    /// Gets or sets the call site.
    /// </summary>
    /// <value>
    /// The call site.
    /// </value>
    public string CallSite { get; set; }

    /// <summary>
    /// Gets or sets the exception.
    /// </summary>
    /// <value>
    /// The exception.
    /// </value>
    public string Exception { get; set; }
}