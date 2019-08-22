using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Microsoft.Extensions.Options;
using TimeFlyTrap.Common;
using TimeFlyTrap.Monitoring;
using TimeFlyTrap.WpfApp.Domain.Services;

// ReSharper disable PossibleInvalidOperationException

namespace TimeFlyTrap.WpfApp.Services
{
    public class ApiUploader : IApiUploader
    {
        private readonly IOptions<ApiUploaderOptions> _options;
        private readonly ITokenProvider _tokenProvider;

        private readonly HttpClient _httpClient;
        private readonly ConcurrentQueue<OnActiveWindowInfoEvent> _queuedEvents;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Thread _uploaderThread;

        public ApiUploader(
            IOptions<ApiUploaderOptions> options,
            ITokenProvider tokenProvider)
        {
            _options = options;
            _tokenProvider = tokenProvider;

            _httpClient = new HttpClient
            {
                BaseAddress = options.Value.ApiBaseUrl ?? throw new ArgumentNullException(nameof(options.Value.ApiBaseUrl)),
            };
            _queuedEvents = new ConcurrentQueue<OnActiveWindowInfoEvent>();

            _uploaderThread = new Thread(BackgroundContinualUpload);
            _uploaderThread.Start();
        }

        private TokenInfo Token => _tokenProvider.GetToken();

        private void BackgroundContinualUpload()
        {
            Thread.Sleep(_options.Value.InitialUploadDelay.Value);

            while (true)
            {
                try
                {
                    UploadQueue();
                }
                catch (Exception exception)
                {
                    //TODO: Handle error...
                }
                Thread.Sleep(_options.Value.UploadInterval.Value);
            }
        }

        private void UploadQueue()
        {
            var events = new List<OnActiveWindowInfoEvent>();
            while (_queuedEvents.TryDequeue(out var e))
            {
                events.Add(e);

                if (events.Count > _options.Value.MaxEventCount.Value)
                {
                    break;
                }
            }

            if (events.Count == 0)
            {
                return;
            }

            // No retry?
            var retryCount = 0;
            while (retryCount++ < 10)
            {
                try
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);
                    var result = _httpClient.PostAsJsonAsync("api/v1/TimeFlyTrap/Monitoring", events).GetAwaiter().GetResult();
                    break;
                }
                catch (Exception exception)
                {
                    //TODO: What now?
                }
            }
        }

        public void OnActiveWindowInfo(OnActiveWindowInfoEvent @event)
        {
            _queuedEvents.Enqueue(@event);
        }
    }
}