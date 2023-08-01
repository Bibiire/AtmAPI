namespace Application.DTOs
{
    public class PaginationQuery
    {
        public PaginationQuery()
        {
            PageNumber = 1;
            PageSize = 20;
        }

        public PaginationQuery(int pageNumber, int pageSize, string searchText)
        {
            PageNumber = pageNumber;
            PageSize = pageSize > 20 ? 20 : pageSize;
            SearchText = searchText;
        }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SearchText { get; set; }
    }
}
