using Domain.IService;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Infrastructure.ViewModel.Room;
using Infrastructure.ViewModel;
using Infrastructure.Enum;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.ViewModel.RoomType;

namespace Domain.Service
{
    public class RoomService : IRoomService
    {
        private readonly IRepository<Room> _repository;
        private readonly IRepository<OrderDetail> _orderDetailRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoomService(IRepository<Room> repository, IUnitOfWork unitOfWork, IRepository<OrderDetail> orderDetailRepository)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<IList<Room>> GetListByPageWithOrderPropAndFilterSearchText(PagingListPropModel pagingListPropModel)
        {
            var rooms = pagingListPropModel.SearchText.IsNullOrEmpty()
                  ? await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(
                                            pagingListPropModel, 
                                            include: s => s.Include(s => s.Department))
                                        .ConfigureAwait(true)
                  : await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(pagingListPropModel: pagingListPropModel, 
                                            predicate: s => s.Name.Contains(pagingListPropModel.SearchText), 
                                            include: s => s.Include(s => s.Department))
                                        .ConfigureAwait(true);
            return rooms.ToList();
        }
        public int GetTotal(PagingListPropModel pagingListPropModel)
        {
            var total = pagingListPropModel.SearchText.IsNullOrEmpty() ? _repository.GetTotalEntity() : _repository.GetTotalEntity(predicate: s => s.Name.Contains(pagingListPropModel.SearchText));
            return total;
        }

        public async Task<Room> GetById(Guid id)
        {
            return await _repository.SingleOrDefaultAsync(predicate: r => r.Id == id, include: r => r.Include(d => d.Department).Include(rt => rt.RoomType)).ConfigureAwait(true);
        }

        public async Task<IList<Room>> GetRoomByDepartmentId(Guid departmentId)
        {
            var rooms = await _repository.GetWithPredicateAsync(r => r.DepartmentId == departmentId).ConfigureAwait(true);
            return rooms.ToList();
        }

        public async Task<IList<Room>> GetRoomByRoomTypeId(Guid roomTypeId)
        {
            var rooms = await _repository.GetWithPredicateAsync(r => r.RoomTypeId == roomTypeId).ConfigureAwait(true);
            return rooms.ToList();
        }

        public async Task<IList<Room>> GetRoomsAvailableInDepartmentForRoomType(FilterRoom filterRoom)
        {
            var rooms = new List<Room>();

            var roomsInDepartment = await _repository.GetAllAsync(predicate: r => r.RoomType.MaxPerson >= filterRoom.MaxPerson && r.DepartmentId == filterRoom.DepartmentId && r.Status != StatusRoomEnum.Repair.ToString() && r.IsActive == true).ConfigureAwait(true);

            var roomInDepartmentIds = roomsInDepartment.Select(r => r.Id).ToList();

            var roomsBookedInOrder = await _orderDetailRepository.GetAllAsync(predicate: od => roomInDepartmentIds.Contains(od.OriginalId) && od.Order.Status != StatusOrderEnum.Delete.ToString() && od.IsRoom == true).ConfigureAwait(true);

            var roomBookedInOrderIds = roomsBookedInOrder.Select(s => s.OriginalId).Distinct().ToList();
            
            var roomAvailableIds = GetAvailableRooms(roomsBookedInOrder.ToList(), filterRoom.StartDate, filterRoom.EndDate);
            
            if (roomInDepartmentIds.Count > roomBookedInOrderIds.Count)
            {
                var roomsNeverBook = roomInDepartmentIds.Where(r => !roomBookedInOrderIds.Contains(r)).ToList();
                roomAvailableIds.AddRange(roomsNeverBook);
            }

            rooms = roomsInDepartment.Where(s => roomAvailableIds.Contains(s.Id)).ToList();

            return rooms;
        }

        public async Task<Room> GetRoomAvailableInDepartmentForBooking(FilterRoom filterRoom)
        {
            Guid roomAvailableId = Guid.Empty;

            var roomsByDepart = await _repository.GetAllAsync(predicate: r => r.RoomTypeId == filterRoom.RoomTypeId && r.DepartmentId == filterRoom.DepartmentId && r.Status != StatusRoomEnum.Repair.ToString() && r.IsActive == true).ConfigureAwait(true);

            var roomByDepartIds = roomsByDepart.Select(r => r.Id).ToList();

            var roomBookedInOrder = await _orderDetailRepository.GetAllAsync(predicate: od => roomByDepartIds.Contains(od.OriginalId) && od.Order.Status != StatusOrderEnum.Delete.ToString() && od.IsRoom).ConfigureAwait(true);

            var roomBookedInOrderIds = roomBookedInOrder.Select(s => s.OriginalId).Distinct().ToList();

            var rnd = new Random();

            var roomAvailableIds = GetAvailableRooms(roomBookedInOrder.ToList(), filterRoom.StartDate, filterRoom.EndDate);

            var randomRoomId = rnd.Next(roomAvailableIds.Count);

            if (roomAvailableIds.Any())
            {
                roomAvailableId = randomRoomId <= 0 ? roomAvailableIds[0] : roomAvailableIds[randomRoomId - 1];
            }

            
            if (roomAvailableId == Guid.Empty)
            {
                if(roomByDepartIds.Count > roomBookedInOrderIds.Count)
                {
                    roomAvailableId = roomByDepartIds.Where(r => !roomBookedInOrderIds.Contains(r)).FirstOrDefault();
                }
                else
                    return null;
            }

            var roomAvailable = roomsByDepart.Where(s => s.Id == roomAvailableId).FirstOrDefault();

            return roomAvailable;
        }

        public async Task<Guid> Add(Room room)
        {
            var roomed = await _repository.FirstOrDefaultAsync(predicate: rt => rt.Name == room.Name).ConfigureAwait(true);

            if (roomed != null)
            {
                return Guid.Empty;
            }

            _repository.Add(room);
            await _unitOfWork.CommitChangesAsync();

            return room.Id;
        }

        public async Task<string> Update(Room roomUpdate)
        {
            var roomed = await _repository.FirstOrDefaultAsync(predicate: rt => rt.Name == roomUpdate.Name && rt.Id != roomUpdate.Id).ConfigureAwait(true);

            if (roomed != null)
            {
                return ResponseEnum.NameIsExisted.ToString();
            }

            var room = await _repository.SingleOrDefaultAsync(predicate: l => l.Id == roomUpdate.Id).ConfigureAwait(true);

            if(room == null)
            {
                return ResponseEnum.SomewhereWrong.ToString();
            }

            room.UpdateDate = DateTime.UtcNow;
            room.IsActive = roomUpdate.IsActive;
            room.Status = roomUpdate.Status;

            room.Name = roomUpdate.Name;
            room.DepartmentId = roomUpdate.DepartmentId;
            room.RoomTypeId = roomUpdate.RoomTypeId;

            await _unitOfWork.CommitChangesAsync().ConfigureAwait(true);

            return ResponseEnum.Success.ToString();
        }

        private List<Guid> GetAvailableRooms(List<OrderDetail> bookedList, DateTime startDate, DateTime endDate)
        {
            // Filter rooms that are not booked during the user's requested period
            var availableRooms = bookedList
                .GroupBy(b => b.OriginalId)
                .Where(group => group.All(b =>
                    endDate <= b.StartDate || startDate >= b.EndDate))
                .Select(group => group.Key)
                .ToList();

            return availableRooms;
        }

        public async Task UpdateStatusToOrder(Guid roomId, string status)
        {
            var room = await _repository.SingleOrDefaultAsync(predicate: s=>s.Id == roomId).ConfigureAwait(true);
            room.Status = status;
            await _unitOfWork.CommitChangesAsync();
        }

        #region /*Do not use regularly*/
        public async Task<IList<Room>> GetAll(bool orderDesByCreate = false)
        {
            var temp = _repository.GetAllAsync(orderDesByCreate: orderDesByCreate, include: r => r.Include(d => d.Department)).Result.ToList();
            return temp;
        }

        public async Task<string> Remove(Guid id)
        {
            var room = await _repository.SingleOrDefaultAsync(predicate: l => l.Id == id).ConfigureAwait(true);

            if (room == null)
            {
                return ResponseEnum.SomewhereWrong.ToString();
            }

            _repository.Remove(room);
            await _unitOfWork.CommitChangesAsync().ConfigureAwait(true);

            return ResponseEnum.Success.ToString();
        }


        #endregion
    }

}

