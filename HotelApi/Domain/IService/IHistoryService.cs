using Infrastructure.Entities;
using Infrastructure.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService
{
    public interface IHistoryService
    {
        Task<Guid> Add(History history);
        Task<IList<History>> GetAll(bool orderDesByCreate = false);
        Task<History> GetById(Guid id);
        Task<History> Update(History historyUpdate);
        Task<string> Remove(Guid id);
        int GetTotal(PagingListPropModel pagingListPropModel);
        Task<IList<History>> GetListByPageWithOrderPropAndFilterSearchText(PagingListPropModel pagingListPropModel);
        Task<IList<History>> GetByOrderId(Guid orderId);
    }
}
