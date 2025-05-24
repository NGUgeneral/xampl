namespace xampl.ViewModels
{
    public class DocumentVM
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public string Title { get; set; } = string.Empty;

        public List<DocumentListVM> DocumentLists { get; set; } = [];

        public List<DocumentNoteVM> DocumentNotes { get; set; } = [];
    }
}
