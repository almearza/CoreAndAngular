namespace API.Helpers
{
    public class PaginationHeader
    {
        public PaginationHeader(int totalItems, int totalpages, int currentPage, int itemsPerPage)
        {
            TotalItems = totalItems;
            Totalpages = totalpages;
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
        }

        public int TotalItems { get; set; }
        public int Totalpages { get; set; }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
    }
}