namespace xampl.ViewModels
{
    public class DocumentListVM
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public List<DocumentListItemVM> DocumentListItems { get; set; } = [];

        public short Position { get; set; }
    }
}
