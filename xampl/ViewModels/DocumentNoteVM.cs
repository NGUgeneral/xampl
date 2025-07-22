namespace xampl.ViewModels
{
    public class DocumentNoteVM
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public string Text { get; set; } = string.Empty;

        public short Position { get; set; }

        public int DocumentId { get; set; }
    }
}
