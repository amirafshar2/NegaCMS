namespace BE
{
    public class Comment
    {
        public int Id { get; set; }
        public string AuthorName { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        /// <summary>true = approved and visible on the website.</summary>
        public bool Status { get; set; }

        public int BlogId { get; set; }
        public Blog Blog { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public List<Reply> Replies { get; set; } = new();
    }
}
