using Domain.IService;
using Infrastructure.Entities;
using Infrastructure.ViewModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Service
{
    public class HelperService<T> : IHelperService<T>
    {
        private readonly ILogger<HelperService<T>> _logger;
        public HelperService(ILogger<HelperService<T>> logger)
        {
            _logger = logger;
        }
        public bool IsValidProperty(string propertyName)
        {
            // You can customize this method to check if propertyName is a valid property of the Department class
            return typeof(T).GetProperty(propertyName) != null;
        }

        public PropertyInfo GetPropertyInfo(string propertyName)
        {
            var propertyInfo = typeof(User).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return propertyInfo;
        }

        public Func<T, object> GetOrderByExpression(string orderProperty)
        {
            // Use reflection to dynamically create a lambda expression for ordering by the specified property
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, orderProperty);
            var conversion = Expression.Convert(property, typeof(object)); // Convert to object for comparison
            var lambda = Expression.Lambda<Func<T, object>>(conversion, parameter);


            return lambda.Compile();
        }

        public DateTime ChangeUTCTimeZone(DateTime inputDateTime)
        {
            _logger.LogInformation(inputDateTime.ToString());
            // Get the local time zone
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;

            // Get the current UTC time
            DateTime utcNow = DateTime.UtcNow;

            // Convert UTC time to local time
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, localTimeZone);

            // Calculate the difference between local time and UTC time
            TimeSpan difference = localTime - utcNow;

            inputDateTime = inputDateTime.AddHours((-1) * difference.TotalHours);

            _logger.LogInformation("Log Timezone check");
            _logger.LogInformation(difference.TotalHours.ToString());
            _logger.LogInformation(inputDateTime.ToString());



            return inputDateTime;
        }
    }
}
