using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using xampl.Services.ConfigOptionsService;

namespace xampl.Services.EmailSenderService
{
    public class EmailSender(IOptions<ConfigOptions> configOptions)
    {
        private readonly ConfigOptions _config = configOptions.Value;

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_config.SmtpSettings.FromName, _config.SmtpSettings.FromEmail));
            email.To.Add(new MailboxAddress("Recipient", toEmail));
            email.Subject = subject;
            //TODO: replace me with HTML support;
            email.Body = new TextPart("plain") { Text = message };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_config.SmtpSettings.Server, _config.SmtpSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_config.SmtpSettings.Username, _config.SmtpSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }


}
