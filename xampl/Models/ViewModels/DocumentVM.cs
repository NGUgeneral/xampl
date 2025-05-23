using xampl.Models.DTO;

namespace xampl.Models.ViewModels
{
    public class DocumentVM
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public string Title { get; set; } = string.Empty;

        public List<DocumentListVM> Lists { get; set; } = [];

        public List<DocumentNote> Notes { get; set; } = [];
    }
}
