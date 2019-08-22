using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TimeFlyTrap.WpfApp.Properties;

namespace TimeFlyTrap.WpfApp
{
    public static class ConfigHelpers
    {
        public static void ConfigureAndValidate<T>(this IServiceCollection serviceCollection, Action<T> configureOptions) where T : class, new()
        {
            // Inspired by https://blog.bredvid.no/validating-configuration-in-asp-net-core-e9825bd15f10
            serviceCollection.Configure(configureOptions);

            using (var scope = serviceCollection.BuildServiceProvider().CreateScope())
            {
                var options = scope.ServiceProvider.GetRequiredService<IOptions<T>>();
                var optionsValue = options.Value;
                ForceValidateObject(optionsValue);
            }
        }

        private static void ForceValidateObject(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), string.Format(Resources.Object_is_required_in__0_, nameof(ForceValidateObject)));
            }

            var configErrors = ValidationErrors(obj).ToArray();
            if (!configErrors.Any())
            {
                return;
            }

            var aggregatedErrors = string.Join(",", configErrors);
            var count = configErrors.Length;
            var configType = obj.GetType().FullName;
            throw new ApplicationException($"{configType} configuration has {count} error(s): {aggregatedErrors}");
        }

        private static IEnumerable<string> ValidationErrors(object obj)
        {
            var context = new ValidationContext(obj, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(obj, context, results, true);
            foreach (var validationResult in results)
            {
                yield return validationResult.ErrorMessage;
            }
        }
    }
}