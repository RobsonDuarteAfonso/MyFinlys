namespace MyFinlys.Application.DTOs;

public class PaginationParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public void Validate()
    {
        if (PageNumber < 1)
            PageNumber = 1;

        if (PageSize < 1)
            PageSize = 10;

        if (PageSize > 100)
            PageSize = 100; // Máximo de 100 itens por página
    }
}

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
