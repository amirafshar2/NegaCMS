namespace BE
{
    /// <summary>Counter shown on the home page (e.g. "120 finished projects").</summary>
    public class Success
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public int TotalSuccess { get; set; }
        public string Suffix { get; set; }
        public int Order { get; set; }
        public bool Status { get; set; } = true;
    }
}
