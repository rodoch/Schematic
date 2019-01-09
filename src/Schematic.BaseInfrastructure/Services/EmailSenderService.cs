using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Ansa.Extensions;
using Schematic.Core;

namespace Schematic.BaseInfrastructure
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IOptionsMonitor<SchematicSettings> _settings;
        private readonly EmailSettings _emailSettings;

        public EmailSenderService(IOptionsMonitor<SchematicSettings> settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _emailSettings = _settings.CurrentValue.Email;
        }

        private SmtpClient GetClient()
        {
            var client = new SmtpClient();

            if (_emailSettings.SMTPCredentials != null) 
            {
                client.Credentials = _emailSettings.SMTPCredentials;
            }

            if (_emailSettings.SMTPHost.HasValue()) 
            {
                client.Host = _emailSettings.SMTPHost;
            }

            if (_emailSettings.SMTPPort > 0) 
            {
                client.Port = _emailSettings.SMTPPort.Value;
            }

            if (_emailSettings.SMTPEnableSSL) 
            {
                client.EnableSsl = true;
            }

            return client;
        }

        public Task SendEmailAsync(string email, string subject, string body)
        {
            using (var message = new MailMessage())
            {   
                message.To.Add(email);

                if (_emailSettings.FromMailAddress != null) 
                {
                    message.From = _emailSettings.FromMailAddress;
                }

                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using (var client = GetClient())
                {
                    client.Send(message);
                }
            }

            return Task.CompletedTask;
        }
    }
}