using System.Web;

namespace Schematic.Core.Mvc
{
    public class AuthenticationViewModel
    {
        public string Mode { get; set; }

        private string _token;
        public string Token 
        { 
            get => _token;
            set => _token = HttpUtility.UrlEncode(value);
        }
    }
}