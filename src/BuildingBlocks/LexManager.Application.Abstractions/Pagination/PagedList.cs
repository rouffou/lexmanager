namespace LexManager.Application.Abstractions.Pagination;

/// <summary>
/// A single page of results plus the metadata a client needs to navigate.
/// Returned as a DTO from queries — domain entities never cross this boundary.
/// </summary>
public sealed record PagedList<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public static PagedList<T> Empty(PaginationParameters parameters) =>
        new([], parameters.Page, parameters.PageSize, 0);
}
