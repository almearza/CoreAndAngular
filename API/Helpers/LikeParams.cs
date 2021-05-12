namespace API.Helpers
{
    public class LikeParams:PaginationParams
    {
        public int userId { get; set; }
        public string predicate { get; set; }
    }
}