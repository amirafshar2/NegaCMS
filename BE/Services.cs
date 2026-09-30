namespace BE
{
    public class Services
    {
        public int Id { get; set; }
        public string Title { get; set; }
        /// <summary>Font Awesome class, e.g. "fas fa-code".</summary>
        public string Icon { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        public bool Status { get; set; } = true;
    }
}
