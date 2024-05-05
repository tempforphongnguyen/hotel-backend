using Infrastructure.Entities;
using Infrastructure.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService
{
    public interface INotificationService
    {
        Task<IList<Notification>> GetListByPageWithOrderPropAndFilterSearchText(FilterNotification filterNotification);
        Task<bool> CheckNotiUnRead(Guid userId);
        Task Add(Notification notification);
        Task Update(Notification notificationUpdate);
        Task AutoCreateWithType(Enum type, string key, Guid userId);
    }
}
