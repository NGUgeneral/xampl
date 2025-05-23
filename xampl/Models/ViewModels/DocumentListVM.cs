using xampl.Models.DTO;

namespace xampl.Models.ViewModels
{
    public class DocumentListVM
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public List<DocumentListItemVM> ListItems { get; set; } = [];

        public short Position { get; set; }
    }
}
