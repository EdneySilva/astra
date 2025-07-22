namespace Astra.Domain.Abstractions
{
    public class PageResult<TItem>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItemsFound { get; set; }
        public IEnumerable<TItem>? Items { get; set; }
    }

    public struct PageRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }

        public static PageRequest First(int pageSize = 10)
        {
            return new PageRequest { Page = 1, PageSize = pageSize };
        }
    }
}
