using Domain.IService;
using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Infrastructure.ViewModel;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Core.Types;
using Infrastructure.ViewModel.RoomType;
using Infrastructure.ViewModel.Auth.User;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Enum;
using Infrastructure.ViewModel.Order;
using System.Globalization;

namespace Domain.Service
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IRepository<RoomType> _repository;
        private readonly IImageRoomTypeService _imageRoomTypeService;
        private readonly IRoomService _roomService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUploadFileService _uploadFileService;

        public RoomTypeService(IRepository<RoomType> repository, 
                               IUnitOfWork unitOfWork, 
                               IRoomService roomService, 
                               IUploadFileService uploadFileService,
                               IImageRoomTypeService imageRoomTypeService
                              )
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _roomService = roomService;
            _uploadFileService = uploadFileService;
            _imageRoomTypeService = imageRoomTypeService;
        }

        public async Task<IList<RoomType>> GetListByPageWithOrderPropAndFilterSearchText(PagingListPropModel pagingListPropModel)
        {
            var roomTypes = pagingListPropModel.SearchText.IsNullOrEmpty()
                            ? await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(pagingListPropModel: pagingListPropModel, include: rt => rt.Include(irt => irt.ImageRoomTypes)).ConfigureAwait(true)
                            : await _repository.GetWithPredicateAndPagingListWithOrderPropAsync(pagingListPropModel: pagingListPropModel, include: rt => rt.Include(irt => irt.ImageRoomTypes), predicate: s => s.Name.Contains(pagingListPropModel.SearchText)).ConfigureAwait(true);

            return roomTypes.ToList();
        }
        
        public int GetTotal(PagingListPropModel pagingListPropModel)
        {
            var total = pagingListPropModel.SearchText.IsNullOrEmpty() ? _repository.GetTotalEntity() : _repository.GetTotalEntity(predicate: s => s.Name.Contains(pagingListPropModel.SearchText));
            return total;
        }

        public async Task<RoomType> GetById(Guid id)
        {
            return await _repository.SingleOrDefaultAsync(predicate: s => s.Id == id, include: rt => rt.Include(irt => irt.ImageRoomTypes)).ConfigureAwait(true);
        }

        public async Task<IList<RoomType>> GetRoomTypeByDepartmentId(Guid departmentId)
        {
            var room = await _roomService.GetRoomByDepartmentId(departmentId);
            
            if (room == null)
            {
                return null;
            }
            var roomTypeIds = room.Select(x => x.RoomTypeId).Distinct().ToList();

            var roomType = await _repository.GetWithPredicateAndPagingListAsync(predicate: rt => roomTypeIds.Contains(rt.Id)).ConfigureAwait(true);

            return roomType.ToList();
        }

        public async Task<Guid> Add(RoomType roomType)
        {
            var roomTyped = await _repository.FirstOrDefaultAsync(predicate: rt => rt.Name == roomType.Name).ConfigureAwait(true);

            if(roomTyped != null)
            {
                return Guid.Empty;
            }

            foreach(var image in roomType.ImageRoomTypes)
            {
                image.RoomTypeId = roomType.Id;
            }

            _repository.Add(roomType);
            await _unitOfWork.CommitChangesAsync();

            return roomType.Id;
        }

        public async Task<string> Update(RoomType roomTypeUpdate)
        {
            var roomTyped = await _repository.FirstOrDefaultAsync(predicate: s=>s.Name == roomTypeUpdate.Name && s.Id != roomTypeUpdate.Id).ConfigureAwait(true);
            if (roomTyped != null)
            {
                return ResponseEnum.NameIsExisted.ToString();
            }
            var roomType = await _repository.SingleOrDefaultAsync(predicate: l => l.Id == roomTypeUpdate.Id, include: rt=>rt.Include(rti => rti.ImageRoomTypes), false).ConfigureAwait(true);
            if (roomType == null)
            {
                return ResponseEnum.SomewhereWrong.ToString();
            }

            roomType.UpdateDate = DateTime.UtcNow;
            roomType.IsActive = roomTypeUpdate.IsActive;
            roomType.Status = roomTypeUpdate.Status;

            roomType.Description = roomTypeUpdate.Description;
            roomType.Name = roomTypeUpdate.Name;
            roomType.Price = roomTypeUpdate.Price;
            roomType.MaxPerson = roomTypeUpdate.MaxPerson;
            roomType.MaxChild = roomTypeUpdate.MaxChild;
            roomType.View = roomTypeUpdate.View;
 

            var fileNameAdds = roomTypeUpdate.ImageRoomTypes.Select(s=>s.FileName).Where(s=> !roomType.ImageRoomTypes.Select(s=>s.FileName).Contains(s)).ToList();
            var fileNameRemoves = roomType.ImageRoomTypes.Select(s => s.FileName).Where(s => !roomTypeUpdate.ImageRoomTypes.Select(s => s.FileName).Contains(s)).ToList();

            if (fileNameAdds.Any())
            {
                var imageRoomTypeAdds = new List<ImageRoomType>();
                foreach(var imageAdd in fileNameAdds)
                {
                    imageRoomTypeAdds.Add(new ImageRoomType()
                    {
                        FileName = imageAdd,
                        RoomTypeId = roomType.Id,
                    });
                }
                await _imageRoomTypeService.AddImageRoomTypes(imageRoomTypeAdds);
            }

            if (fileNameRemoves.Any())
            {
                var imageRoomTypeRemoves = new List<ImageRoomType>();
                foreach (var imageRemove in fileNameRemoves)
                {
                    imageRoomTypeRemoves.Add(new ImageRoomType()
                    {
                        FileName = imageRemove,
                        RoomTypeId = roomType.Id,
                    });
                }
                var result = await _imageRoomTypeService.RemoveImageRoomTypes(imageRoomTypeRemoves);
            }

            _repository.Update(roomType);
            await _unitOfWork.CommitChangesAsync().ConfigureAwait(true);

            return ResponseEnum.Success.ToString();
        }

        public async Task<IList<RoomType>> GetRoomTypesAvailableInDepartment(FilterRoom filterRoom)
        {
            filterRoom.EndDate = DateTime.Parse(filterRoom.EndDate.ToString(), null, DateTimeStyles.AdjustToUniversal).AddHours(12);
            filterRoom.StartDate = DateTime.Parse(filterRoom.StartDate.ToString(), null, DateTimeStyles.AdjustToUniversal).AddHours(14);

            var roomsAvailable = await _roomService.GetRoomsAvailableInDepartmentForRoomType(filterRoom);

            var roomTypesAvailableId = roomsAvailable.Select(s => s.RoomTypeId).Distinct();

            var roomTypes = await _repository.GetAllAsync(predicate: rt => roomTypesAvailableId.Contains(rt.Id), include: s=>s.Include(irt => irt.ImageRoomTypes)).ConfigureAwait(true);

            return roomTypes.ToList();
        }

        #region /*Do not use regularly*/

        public async Task<IList<RoomType>> GetAll(bool orderDesByCreate = false)
        {
            var roomTypes = await _repository.GetAllAsync(orderDesByCreate: orderDesByCreate).ConfigureAwait(true);
            return roomTypes.ToList();
        }

        public async Task<string> Remove(Guid id)
        {
            var roomType = await _repository.SingleOrDefaultAsync(predicate: l => l.Id == id).ConfigureAwait(true);

            if (roomType == null)
            {
                return null;
            }

            _repository.Remove(roomType);
            await _unitOfWork.CommitChangesAsync().ConfigureAwait(true);

            return "Success";
        }

        #endregion

    }
}
