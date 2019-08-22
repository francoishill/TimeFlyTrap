using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TimeFlyTrap.Common;
using TimeFlyTrap.WpfApp.Domain.Services;

namespace TimeFlyTrap.WpfApp.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IOptions<TokenProviderOptions> _options;

        private TokenInfo _storedToken;

        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public TokenProvider(
            IOptions<TokenProviderOptions> options)
        {
            _options = options;
        }

        public TokenInfo GetToken()
        {
            const int toleranceMinutes = 5;

            if (_storedToken != null && _storedToken.AccessTokenExpiration > DateTime.Now.AddMinutes(toleranceMinutes))
            {
                return _storedToken;
            }

            _semaphore.Wait();
            try
            {
                if (_storedToken != null && _storedToken.AccessTokenExpiration > DateTime.Now.AddMinutes(toleranceMinutes))
                {
                    return _storedToken;
                }

                _storedToken = LoadToken();

                if (_storedToken != null)
                {
                    if (_storedToken.AccessTokenExpiration > DateTime.Now.AddMinutes(toleranceMinutes))
                    {
                        return _storedToken;
                    }

                    DeleteToken();
                    _storedToken = null;
                }

                var token = AskUserForToken();
                SaveToken(token);
                _storedToken = token;
                return token;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private static string TokenFilePath => Path.Combine(AppStateHelper.ApplicationLocalDataDirectory, "token.json");

        private static void SaveToken(TokenInfo token)
        {
            var json = JsonConvert.SerializeObject(token);
            File.WriteAllText(TokenFilePath, json);
        }

        private static TokenInfo LoadToken()
        {
            if (!File.Exists(TokenFilePath))
            {
                return null;
            }

            var json = File.ReadAllText(TokenFilePath);
            return JsonConvert.DeserializeObject<TokenInfo>(json);
        }

        private static void DeleteToken()
        {
            File.Delete(TokenFilePath);
        }

        private TokenInfo AskUserForToken()
        {
            var flags = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("--authority", _options.Value.Authority),
                new KeyValuePair<string, string>("--client-id", _options.Value.ClientId),
                new KeyValuePair<string, string>("--client-secret", _options.Value.ClientSecret),
            };

            //TODO: Rather pass the credentials via StdIn

            //TODO: How to get this path?
            //var exePath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\TimeFlyTrap.AuthConsole\bin\Release\netcoreapp2.2\win10-x64\TimeFlyTrap.AuthConsole.exe");
            //var startInfo = new ProcessStartInfo(exePath, string.Join(" ", flags.Select(x => $"{x.Key}={x.Value}")))
            var dllPath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\TimeFlyTrap.AuthConsole\bin\Debug\netcoreapp2.2\TimeFlyTrap.AuthConsole.dll");
            var startInfo = new ProcessStartInfo("dotnet", $"\"{dllPath}\" " + string.Join(" ", flags.Select(x => $"{x.Key}={x.Value}")))
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
            };

            var process = new Process
            {
                StartInfo = startInfo,
            };

            var stdOut = new StringBuilder();
            var stdErr = new StringBuilder();
            process.OutputDataReceived += (_, e) => stdOut.Append(e?.Data ?? "");
            process.ErrorDataReceived += (_, e) => stdErr.Append(e?.Data ?? "");

            if (!process.Start())
            {
                throw new Exception("Unable to start process");
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            const int waitMinutes = 5;
            if (!process.WaitForExit(waitMinutes * 60 * 1000))
            {
                // we timed out...
                process.Kill();
                throw new Exception($"Unable to wait for process. StdOut: {stdOut}. StdErr: {stdErr}");
            }

            if (process.ExitCode != 0)
            {
                throw new Exception($"Process failed with exit code {process.ExitCode}. StdOut: {stdOut}. StdErr: {stdErr}");
            }

            var tokenInfo = ExtractTokenInfoFromStdOut(stdOut.ToString());
            return tokenInfo;
        }

        private static TokenInfo ExtractTokenInfoFromStdOut(string stdOut)
        {
            var stdOutPattern = new Regex(@"\[\[START_RESULT\]\](.*)\[\[END_RESULT\]\]");
            var match = stdOutPattern.Match(stdOut);
            if (!match.Success)
            {
                throw new Exception($"Process completed successfully but StdOut did not match pattern {stdOutPattern}");
            }

            var json = match.Groups[1].Value;
            return JsonConvert.DeserializeObject<TokenInfo>(json);
        }
    }
}