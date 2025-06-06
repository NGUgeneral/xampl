#nullable disable
namespace xampl.Services.ConfigOptionsService
{
    public class ConfigOptions
    {
        public const string ConfigVariablesSectionKey = "Variables";
        public const string ConfigSmtpSettingsSectionKey = "SmtpSettings";
        public string Domain { get; set; }
        public SmtpSettings SmtpSettings { get; set; } = new SmtpSettings();
    }

    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }
}
