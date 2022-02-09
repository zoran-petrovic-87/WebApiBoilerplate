namespace WebApi.Helpers.Pagination;

/// <summary>
/// Pagination filter.
/// </summary>
public class PaginationFilter
{
    private int _pageNumber;
    private int _pageSize;
    private readonly int _maxPageSize = 25;

    /// <summary>
    /// Gets or sets the page number.
    /// If not provided as URL parameter, it will be set to 1.
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber == 0 ? 1 : _pageNumber;
        set => _pageNumber = value;
    }

    /// <summary>
    /// Gets or sets the page size (number of items per page).
    /// If this value is not provided as URL parameter or it is greater than <c>_maxPageSize</c> it will be
    /// set to <c>_maxPageSize</c>.
    /// </summary>
    public int PageSize
    {
        get => _pageSize > _maxPageSize || _pageSize == 0 ? _maxPageSize : _pageSize;
        set => _pageSize = value;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public PaginationFilter()
    {
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="maxPageSize">The maximum number of items per page.</param>
    public PaginationFilter(int maxPageSize)
    {
        _maxPageSize = maxPageSize;
    }
}