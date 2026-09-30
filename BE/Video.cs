namespace BE
{
    public class Video
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        /// <summary>YouTube link or video id.</summary>
        public string VideoLink { get; set; }
        public string Picture { get; set; }
        public int View { get; set; }
        public bool Status { get; set; } = true;
    }
}
