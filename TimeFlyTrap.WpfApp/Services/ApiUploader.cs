using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TimeFlyTrap.Common;
using TimeFlyTrap.Monitoring;
using TimeFlyTrap.WpfApp.Domain.Services;

// ReSharper disable MemberCanBePrivate.Local

// ReSharper disable UnusedAutoPropertyAccessor.Local

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local

// ReSharper disable PossibleInvalidOperationException

namespace TimeFlyTrap.WpfApp.Services
{
    public class ApiUploader : IApiUploader, IDisposable
    {
        private readonly IOptions<ApiUploaderOptions> _options;
        private readonly ILogger<ApiUploader> _logger;
        private readonly ITokenProvider _tokenProvider;

        private readonly HttpClient _httpClient;
        private readonly ConcurrentQueue<RecordingEvent> _queuedEvents;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Thread _uploaderThread;
        private DateTime _previousTime = DateTime.Now;
        private bool _abort;

        public ApiUploader(
            IOptions<ApiUploaderOptions> options,
            ILogger<ApiUploader> logger,
            ITokenProvider tokenProvider)
        {
            _options = options;
            _logger = logger;
            _tokenProvider = tokenProvider;

            _httpClient = new HttpClient
            {
                BaseAddress = options.Value.ApiBaseUrl ?? throw new ArgumentNullException(nameof(options.Value.ApiBaseUrl)),
            };
            _queuedEvents = new ConcurrentQueue<RecordingEvent>();

            _uploaderThread = new Thread(BackgroundContinualUpload);
            _uploaderThread.Start();
        }

        private TokenInfo Token => _tokenProvider.GetToken();

        private void BackgroundContinualUpload()
        {
            Thread.Sleep(_options.Value.InitialUploadDelay.Value);

            while (!_abort)
            {
                try
                {
                    UploadQueue();
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Unable to upload queue");
                }
                Thread.Sleep(_options.Value.UploadInterval.Value);
            }
        }

        private void UploadQueue()
        {
            var events = new List<RecordingEvent>();
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

            var retryCount = -1;
            while (retryCount++ < 10)
            {
                try
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

                    var postBody = new UploadRequest(events);

                    var result = _httpClient.PostAsJsonAsync("api/v1/TimeFlyTrap/Monitoring", postBody).GetAwaiter().GetResult();
                    result.EnsureSuccessStatusCode();
                    break;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Upload failed, retryCount={retryCount}");
                }
            }
        }

        public void OnActiveWindowInfo(OnActiveWindowInfoEvent @event)
        {
            var now = DateTime.Now;
            var diff = now.Subtract(_previousTime);
            _previousTime = now;

            _queuedEvents.Enqueue(new RecordingEvent(@event, diff));
        }

        private class RecordingEvent
        {
            [JsonProperty("time")]
            public DateTime Time { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("module_file_path")]
            public string ModuleFilePath { get; set; }

            [JsonProperty("system_startup_time")]
            public DateTime SystemStartupTime { get; set; }

            [JsonProperty("user_idle_duration")]
            public TimeSpan UserIdleDuration { get; set; }

            [JsonProperty("time_diff")]
            public TimeSpan TimeDiff { get; set; }

            public RecordingEvent(OnActiveWindowInfoEvent @event, TimeSpan timeDiff)
            {
                Time = @event.Time;
                Title = @event.Title;
                ModuleFilePath = @event.ModuleFilePath;
                SystemStartupTime = @event.SystemStartupTime;
                UserIdleDuration = @event.UserIdleDuration;
                TimeDiff = timeDiff;
            }
        }

        private class UploadRequest
        {
            public List<RecordingEvent> Events { get; }

            public UploadRequest(List<RecordingEvent> events)
            {
                Events = events;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
            _abort = true;
        }
    }
}