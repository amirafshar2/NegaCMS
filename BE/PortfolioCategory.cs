namespace BE
{
    public class PortfolioCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; } = true;
        public List<Portfolio> Portfolios { get; set; } = new();
    }
}
