using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService
{
    public interface IHelperService<T>
    {
        bool IsValidProperty(string propertyName);
        Func<T, object> GetOrderByExpression(string orderProperty);
        PropertyInfo GetPropertyInfo(string propertyName);
        public DateTime ChangeUTCTimeZone(DateTime inputDateTime);
    }
}
