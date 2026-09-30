namespace BE
{
    /// <summary>Customer testimonial shown on the home page.</summary>
    public class CustomerComment
    {
        public int Id { get; set; }
        public string NameSurname { get; set; }
        public string Brand { get; set; }
        public string Picture { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; } = 5;
        public DateTime Date { get; set; } = DateTime.Now;
        public bool Status { get; set; } = true;
    }
}
