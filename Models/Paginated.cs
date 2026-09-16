namespace BKNova.Models
{
    public class PaginationRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class PaginatedResponse<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public List<T> Data { get; set; } = new();
    }

    public static class PaginationHelper
    {
        public static (int Page, int PageSize) Normalize(int page, int pageSize, int maxPageSize = 100)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > maxPageSize) pageSize = maxPageSize;

            return (page, pageSize);
        }
    }
}
