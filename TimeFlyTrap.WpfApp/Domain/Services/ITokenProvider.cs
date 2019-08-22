using TimeFlyTrap.Common;

namespace TimeFlyTrap.WpfApp.Domain.Services
{
    public interface ITokenProvider
    {
        TokenInfo GetToken();
    }
}