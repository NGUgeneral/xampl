namespace xampl.ViewModels
{
    public class UserVM
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Email { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
    }
}
