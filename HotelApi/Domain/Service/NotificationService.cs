using Domain.IService;
using Infrastructure.Entities;
using Infrastructure.Enum;
using Infrastructure.ViewModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Domain.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IRepository<Notification> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IList<Notification>> GetListByPageWithOrderPropAndFilterSearchText(FilterNotification filterNotification)
        {
            var notifications = await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(filterNotification.PagingListPropModel, predicate: s=> s.UserId == filterNotification.UserId, tracking: false).ConfigureAwait(true);
            
            if(notifications.Any(s=>s.IsRead == false))
            {
                var notiJson = JsonSerializer.Serialize(notifications);
                var notiClones = JsonSerializer.Deserialize<List<Notification>>(notiJson);

                foreach (var noti in notiClones)
                {
                    noti.IsRead = true;
                }

                _repository.Update(notiClones);
                await _unitOfWork.CommitChangesAsync();
            }
   
            return notifications.ToList();
        }

        public async Task<bool> CheckNotiUnRead(Guid userId)
        {
            var unReadNoti = await _repository.CheckHasAnyEntityWithCondition(condition: s => s.UserId == userId && s.IsRead == false).ConfigureAwait(true);
            return unReadNoti;
        }

        public async Task Add(Notification notification)
        {
            _repository.Add(notification);
            await _unitOfWork.CommitChangesAsync();
        }
        
        public async Task Update (Notification notificationUpdate)
        {
            var notification = await _repository.SingleOrDefaultAsync(s=>s.Id == notificationUpdate.Id).ConfigureAwait(true);

            notification.IsRead = notificationUpdate.IsRead;

            await _unitOfWork.CommitChangesAsync();
        }

        public async Task AutoCreateWithType(Enum type, string key, Guid userId)
        {
            var notification = new Notification()
            {
                IsRead = false,
                UserId = userId
            };

            switch(type)
            {
                case CreateNotiTypeEnum.Membership:
                    notification.Title = "You have your membership extended";
                    notification.Description = "Congratulations, your account has been upgraded to " + key + " .Please visit the offers section for detailed information";
                    break;
                case CreateNotiTypeEnum.OrderStatus:
                    notification.Title = "Status room";
                    notification.Description = "Your room status has been " + key +" , if not you please contact hotel right now!";
                    break;
            }

            _repository.Add(notification);
            await _unitOfWork.CommitChangesAsync();
        }


    }
}
