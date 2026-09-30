namespace BE
{
    public class Portfolio
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public string Link { get; set; }
        public bool Status { get; set; } = true;
        public DateTime Date { get; set; } = DateTime.Now;

        public int PortfolioCategoryId { get; set; }
        public PortfolioCategory PortfolioCategory { get; set; }
    }
}
