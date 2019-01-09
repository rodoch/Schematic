using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Schematic.Identity
{
    public class EmailValidatorService : IEmailValidatorService
    {
        private bool _invalid = false;

        public bool IsValidEmail(string email)
        {
            _invalid = false;

            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            // Use IdnMapping class to convert Unicode domain names.
            try 
            {
                email = Regex.Replace(email, @"(@)(.+)$", this.DomainMapper,
                            RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException) 
            {
                return false;
            }

            if (_invalid)
            {
                return false;
            }

            // Return true if strIn is in valid email format.
            try 
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException) 
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();
            string domainName = match.Groups[2].Value;

            try 
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException) 
            {
                _invalid = true;
            }

            return match.Groups[1].Value + domainName;
        }
    }
}