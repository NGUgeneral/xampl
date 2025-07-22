using xampl.Models.Documents;

namespace xampl.ViewModels
{
    public class UserVM
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Source { get; set; } = string.Empty;

        public ICollection<UserRole> UserRoles {  get; set; } = [];

        public List<DocumentVM> Documents { get; set; } = [];
    }
}
