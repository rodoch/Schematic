using System.Net;
using System.Net.Mail;
using Ansa.Extensions;

namespace Schematic.Core
{
    public class EmailSettings
    {
        public MailAddress FromMailAddress { get; private set; }
        public NetworkCredential SMTPCredentials { get; private set; }

        public string ToAddress { get; set; }

        private string _fromAddress;
        public string FromAddress
        {
            get => _fromAddress;
            set
            {
                _fromAddress = value;
                SetMailAddress();
            }
        }

        private string _fromDisplayName;
        public string FromDisplayName
        {
            get => _fromDisplayName;
            set
            {
                _fromDisplayName = value;
                SetMailAddress();
            }
        }

        public string SMTPHost { get; set; }

        public int? SMTPPort { get; set; }

        private string _smtpUserName;
        public string SMTPUserName
        {
            get => _smtpUserName;
            set
            {
                _smtpUserName = value;
                SetCredentials();
            }
        }

        private string _smtpPassword;
        public string SMTPPassword
        {
            get => _smtpPassword;
            set
            {
                _smtpPassword = value;
                SetCredentials();
            }
        }

        public bool SMTPEnableSSL { get; set; }

        private void SetMailAddress()
        {
            try
            {
                FromMailAddress = _fromDisplayName.HasValue()
                    ? new MailAddress(_fromAddress, _fromDisplayName)
                    : new MailAddress(_fromAddress);
            }
            catch
            {
                FromMailAddress = null;
            }
        }

        private void SetCredentials() =>
            SMTPCredentials = _smtpUserName.HasValue() && _smtpPassword.HasValue()
                ? new NetworkCredential(_smtpUserName, _smtpPassword)
                : null;
    }
}