using System;
using System.Collections.Generic;

namespace WebApi.Helpers.Pagination;

/// <summary>
/// Class used to store paginated data and additional information about pages.
/// </summary>
/// <typeparam name="T">Must be a class.</typeparam>
public class PagedResult<T> where T : class
{
    /// <summary>
    /// Gets or sets the current page.
    /// </summary>
    public int CurrentPage { get; set; }
        
    /// <summary>
    /// Gets or sets page count.
    /// </summary>
    public int PageCount { get; set; }
        
    /// <summary>
    /// Gets or sets page size.
    /// </summary>
    public int PageSize { get; set; }
        
    /// <summary>
    /// Gets or sets row count.
    /// </summary>
    public int RowCount { get; set; }
        
    /// <summary>
    /// Gets the number of the first row returned.
    /// </summary>
    public int FirstRowReturned => (CurrentPage - 1) * PageSize + 1;

    /// <summary>
    /// Gets the number of the last row returned.
    /// </summary>
    public int LastRowReturned => Math.Min(CurrentPage * PageSize, RowCount);
        
    /// <summary>
    /// Gets or sets the data.
    /// </summary>
    public IList<T> Data { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public PagedResult()
    {
        Data = new List<T>();
    }
}