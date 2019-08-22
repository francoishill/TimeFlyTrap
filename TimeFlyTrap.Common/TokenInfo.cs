using System;

namespace TimeFlyTrap.Common
{
    public class TokenInfo
    {
        public string AccessToken { get; set; }
        public string IdentityToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public DateTime AuthenticationTime { get; set; }
    }
}