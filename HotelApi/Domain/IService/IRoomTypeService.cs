using Infrastructure.Entities;
using Infrastructure.ViewModel;
using Infrastructure.ViewModel.RoomType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService
{
    public interface IRoomTypeService
    {
        Task<IList<RoomType>> GetAll(bool orderDesByCreate = false);
        Task<RoomType> GetById(Guid id);
        Task<string> Remove(Guid id);
        Task<IList<RoomType>> GetRoomTypeByDepartmentId(Guid departmentId);
        Task<IList<RoomType>> GetListByPageWithOrderPropAndFilterSearchText(PagingListPropModel pagingListPropModel);
        Task<IList<RoomType>> GetRoomTypesAvailableInDepartment(FilterRoom roomTypeAvailableInDepartment);
        int GetTotal(PagingListPropModel pagingListPropModel);
        Task<Guid> Add(RoomType roomType);
        Task<string> Update(RoomType roomTypeUpdate);
    }
}
