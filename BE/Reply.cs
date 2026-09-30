namespace BE
{
    public class Reply
    {
        public int Id { get; set; }
        public string AuthorName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsTeamReply { get; set; }
        public bool Status { get; set; } = true;

        public int CommentId { get; set; }
        public Comment Comment { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
    }
}
