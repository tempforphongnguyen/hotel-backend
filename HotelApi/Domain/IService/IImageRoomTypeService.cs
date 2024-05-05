using Infrastructure.Entities;
using Infrastructure.ViewModel.RoomType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService
{
    public interface IImageRoomTypeService
    {
        Task<string> AddImageRoomType(ImageRoomType imageRoomType);
        Task<IList<ImageRoomType>> GetImageOfRoomTypes(Guid roomTypeId);
        Task<string> RemoveImageRoomType(UpdateImageRoomType updateImageRoomType);
        Task<string> AddImageRoomTypes(IList<ImageRoomType> imageRoomTypes);
        Task<string> RemoveImageRoomTypes(IList<ImageRoomType> imageRoomTypes);
    }
}
