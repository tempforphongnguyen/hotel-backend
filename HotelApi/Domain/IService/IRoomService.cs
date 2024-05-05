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
    public interface IRoomService
    {
        Task<Guid> Add(Room room);
        Task<IList<Room>> GetAll(bool orderDesByCreate = false);
        Task<Room> GetById(Guid id);
        Task<string> Update(Room roomUpdate);
        Task<string> Remove(Guid id);
        Task<IList<Room>> GetRoomByDepartmentId(Guid departmentId);
        Task<IList<Room>> GetRoomByRoomTypeId(Guid roomTypeId);
        Task<IList<Room>> GetListByPageWithOrderPropAndFilterSearchText(PagingListPropModel pagingListPropModel);
        Task<IList<Room>> GetRoomsAvailableInDepartmentForRoomType(FilterRoom filterRoom);
        Task<Room> GetRoomAvailableInDepartmentForBooking(FilterRoom filterRoom);
        int GetTotal(PagingListPropModel pagingListPropModel);
        Task UpdateStatusToOrder(Guid roomId, string status);

    }
}
