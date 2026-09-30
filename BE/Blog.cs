namespace BE
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        /// <summary>Body text. Paragraphs are separated by an empty line, lines starting with "## " are headings.</summary>
        public string Content { get; set; }
        public string Picture { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public bool Status { get; set; } = true;
        public int ViewCount { get; set; }
        public int ReadingMinutes { get; set; } = 3;

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public List<Comment> Comments { get; set; } = new();
    }
}
