using System.ComponentModel.DataAnnotations;

namespace TimeFlyTrap.WpfApp.Services
{
    public class TokenProviderOptions
    {
        [Required]
        public string Authority { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }
    }
}