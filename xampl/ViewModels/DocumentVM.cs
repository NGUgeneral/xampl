namespace xampl.ViewModels
{
    public class DocumentVM
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public int LastUpdatedBy { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content {  get; set; } = string.Empty;
        
        public bool IsPublic { get; set; }
    }
}
