namespace xampl.ViewModels
{
    public class DocumentListItemVM
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Text { get; set; } = string.Empty;

        public bool Checked { get; set; }

        public short Position { get; set; }

        public int CreatedBy { get; set; }
    }
}
