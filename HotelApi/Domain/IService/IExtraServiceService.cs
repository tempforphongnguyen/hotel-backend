using Infrastructure.Entities;
using Infrastructure.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService
{
    public interface IExtraServiceService
    {
        Task<Guid> Add(ExtraService extraService);
        Task<IList<ExtraService>> GetAll(bool orderDesByCreate = false);
        Task<ExtraService> GetById(Guid id);
        Task<ExtraService> Update(ExtraService extraServiceUpdate);
        Task<string> Remove(Guid id);
        Task<IList<ExtraService>> GetListByPageWithOrderPropAndFilterSearchText(PagingListPropModel pagingListPropModel);
        int GetTotal(PagingListPropModel pagingListPropModel);
    }
}
