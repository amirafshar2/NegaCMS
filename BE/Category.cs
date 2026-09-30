namespace BE
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; } = true;
        public List<Blog> Blogs { get; set; } = new();
    }
}
