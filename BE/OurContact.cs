namespace BE
{
    /// <summary>Company information and site settings (one row per language).</summary>
    public class OurContact
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Slogan { get; set; }
        public string HeroTitle { get; set; }
        public string HeroText { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string OpeningHours { get; set; }
        public string Instagram { get; set; }
        public string Telegram { get; set; }
        public string LinkedIn { get; set; }
        public string Github { get; set; }
    }
}
