using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using TimeFlyTrap.Common;

namespace TimeFlyTrap.AuthConsole
{
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "TimeFlyTrap.AuthConsole",
                Description = "Application to get an auth token from an OpenId Connect server"
            };

            app.HelpOption("-?|-h|--help");

            var authority = app.Option("-a|--authority <authority>", "OpenId Connect Authority", CommandOptionType.SingleValue);
            var clientId = app.Option("-i|--client-id <client-id>", "OpenId Connect Client Id", CommandOptionType.SingleValue);
            var clientSecret = app.Option("-s|--client-secret <client-secret>", "OpenId Connect Client Secret", CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                if (!authority.HasValue() || !clientId.HasValue() || !clientSecret.HasValue())
                {
                    app.ShowHint();
                    app.ShowHelp();
                    return 1;
                }

                return ShowBrowserLogin(authority.Value(), clientId.Value(), clientSecret.Value()).GetAwaiter().GetResult();
            });

            return app.Execute(args);
        }

        private static async Task<int> ShowBrowserLogin(string authority, string clientId, string clientSecret)
        {
            try
            {
                var port = GetAvailablePort(9000);

                var options = new OidcClientOptions
                {
                    Authority = authority,
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    Scope = "openid profile email",
                    RedirectUri = $"http://127.0.0.1:{port}",
                    ResponseMode = OidcClientOptions.AuthorizeResponseMode.FormPost,
                    Flow = OidcClientOptions.AuthenticationFlow.AuthorizationCode,
                    Browser = new SystemBrowser(port),
                    Policy = new Policy
                    {
                        RequireAccessTokenHash = false,
                    },
                };

                var oidcClient = new OidcClient(options);

                var result = await oidcClient.LoginAsync(new LoginRequest {BrowserDisplayMode = DisplayMode.Visible});

                if (result.IsError)
                {
                    throw new Exception(result.Error == "UserCancel" ? "The sign-in window was closed before authorization was completed." : result.Error);
                }

                var tokenInfo = new TokenInfo
                {
                    AccessToken = result.AccessToken,
                    IdentityToken = result.IdentityToken,
                    RefreshToken = result.RefreshToken,
                    AccessTokenExpiration = result.AccessTokenExpiration,
                    AuthenticationTime = result.AuthenticationTime,
                };
                var serializedResult = JsonConvert.SerializeObject(tokenInfo);
                await Console.Out.WriteLineAsync("[[START_RESULT]]" + serializedResult + "[[END_RESULT]]");

                await Console.Out.FlushAsync();
                await Console.Error.FlushAsync();

                return 0;
            }
            catch (Exception exception)
            {
                await Console.Error.WriteLineAsync($"Unexpected exception: {exception.Message}");
                return 1;
            }
        }

        private static int GetAvailablePort(int startingPort)
        {
            var properties = IPGlobalProperties.GetIPGlobalProperties();

            //getting active connections
            var tcpConnectionPorts = properties.GetActiveTcpConnections()
                .Where(n => n.LocalEndPoint.Port >= startingPort)
                .Select(n => n.LocalEndPoint.Port);

            //getting active tcp listners - WCF service listening in tcp
            var tcpListenerPorts = properties.GetActiveTcpListeners()
                .Where(n => n.Port >= startingPort)
                .Select(n => n.Port);

            //getting active udp listeners
            var udpListenerPorts = properties.GetActiveUdpListeners()
                .Where(n => n.Port >= startingPort)
                .Select(n => n.Port);

            // ReSharper disable once ReplaceWithSingleCallToFirstOrDefault
            var port = Enumerable.Range(startingPort, ushort.MaxValue)
                .Where(i => !tcpConnectionPorts.Contains(i))
                .Where(i => !tcpListenerPorts.Contains(i))
                .Where(i => !udpListenerPorts.Contains(i))
                .FirstOrDefault();

            return port;
        }
    }
}