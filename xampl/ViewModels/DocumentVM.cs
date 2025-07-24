#nullable disable
using System.ComponentModel.DataAnnotations;
using xampl.Models.Xampl;
using xampl.Validation;

namespace xampl.ViewModels
{
    public class DocumentVM
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public int LastUpdatedBy { get; set; }

        [Required]
        public string Title { get; set; }

        [SanitizeHtml(ErrorMessage = "Your content contains unsafe or disallowed HTML and cannot be saved.")]
        public string Content {  get; set; }
        
        public bool IsPublic { get; set; }

        public virtual User CreatedByNavigation { get; set; }

        public virtual User LastUpdatedByNavigation { get; set; }
    }
}
