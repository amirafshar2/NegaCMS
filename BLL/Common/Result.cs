namespace BLL.Common
{
    /// <summary>Outcome of a business operation with user-readable (German) error messages.</summary>
    public class Result
    {
        public bool Success => Errors.Count == 0;
        public List<string> Errors { get; } = new();

        public static Result Ok() => new();
        public static Result Fail(params string[] errors)
        {
            var r = new Result();
            r.Errors.AddRange(errors);
            return r;
        }
    }

    public class PagedList<T>
    {
        public List<T> Items { get; init; } = new();
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int Total { get; init; }
        public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(Total / (double)PageSize);
    }

    public class DashboardStats
    {
        public int Blogs { get; set; }
        public int Comments { get; set; }
        public int PendingComments { get; set; }
        public int Users { get; set; }
        public int UnreadContacts { get; set; }
        public int Subscribers { get; set; }
        public int Portfolios { get; set; }
        public int Packages { get; set; }
        public int VisitorsToday { get; set; }
        public int VisitorsTotal { get; set; }
        public List<(DateTime Day, int Count)> VisitorsLast14Days { get; set; } = new();
        public List<(string Name, int Count)> BlogsPerCategory { get; set; } = new();
    }
}
