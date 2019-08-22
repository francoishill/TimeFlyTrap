using System;
using System.ComponentModel.DataAnnotations;

namespace TimeFlyTrap.WpfApp.Services
{
    public class ApiUploaderOptions
    {
        [Required]
        public Uri ApiBaseUrl { get; set; }

        [Required]
        public TimeSpan? InitialUploadDelay { get; set; }

        [Required]
        public TimeSpan? UploadInterval { get; set; }

        [Required]
        public int? MaxEventCount { get; set; }
    }
}