namespace BE
{
    /// <summary>A price package (pricing table).</summary>
    public class Package
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string PriceUnit { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        /// <summary>One feature per line.</summary>
        public string Features { get; set; }
        public string Picture { get; set; }
        public int DeliveryDays { get; set; }
        public bool IsPopular { get; set; }
        public int Order { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public bool Status { get; set; } = true;

        public IEnumerable<string> FeatureList =>
            (Features ?? "").Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
