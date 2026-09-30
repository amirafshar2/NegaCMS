namespace BE
{
    public class Notification
    {
        public int Id { get; set; }
        /// <summary>comment | contact | newsletter | user</summary>
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool ReadStatus { get; set; }
    }
}
