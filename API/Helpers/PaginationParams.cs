namespace API.Helpers
{
    public class PaginationParams
    {
        public int PageNumber { get; set; }=1;
        private const int MaxPageSize = 50;
        private int _PageSize = 10;
        public int PageSize
        {
            get => _PageSize;
            set => _PageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        
    }
}