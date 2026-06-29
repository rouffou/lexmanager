namespace LexManager.Application.Abstractions.Pagination;

/// <summary>
/// Standard, clamped pagination input for all list queries. The API is paginated end-to-end
/// (see SRD §5.2), so queries accept this rather than raw page/size integers.
/// </summary>
public sealed record PaginationParameters
{
    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 25;

    public PaginationParameters(int page = 1, int pageSize = DefaultPageSize)
    {
        Page = page < 1 ? 1 : page;
        PageSize = pageSize switch
        {
            < 1 => DefaultPageSize,
            > MaxPageSize => MaxPageSize,
            _ => pageSize
        };
    }

    public int Page { get; }
    public int PageSize { get; }
    public int Skip => (Page - 1) * PageSize;
}
