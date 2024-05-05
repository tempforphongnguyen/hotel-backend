using Domain.IService;
using Infrastructure.Context;
using Infrastructure.Entities;
using Infrastructure.Enum;
using Infrastructure.ViewModel.RoomType;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Service
{
    public class ImageRoomTypeService : IImageRoomTypeService
    {
        private readonly ApplicationDBContext _dbContext;

        public ImageRoomTypeService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> AddImageRoomType(ImageRoomType imageRoomType)
        {
            _dbContext.Set<ImageRoomType>().Add(imageRoomType);
            await _dbContext.SaveChangesAsync();

            return imageRoomType.FileName;
        }

        public async Task<string> AddImageRoomTypes (IList<ImageRoomType> imageRoomTypes)
        {
            await _dbContext.Set<ImageRoomType>().AddRangeAsync(imageRoomTypes);
            await _dbContext.SaveChangesAsync();

            return ResponseEnum.Success.ToString();
        }

        public async Task<string> RemoveImageRoomTypes (IList<ImageRoomType> imageRoomTypes)
        {
            _dbContext.Set<ImageRoomType>().RemoveRange(imageRoomTypes);
            await _dbContext.SaveChangesAsync();
            
            return ResponseEnum.Success.ToString();
        }

        public async Task<IList<ImageRoomType>> GetImageOfRoomTypes(Guid roomTypeId)
        {
            var imageRoomTypes = await _dbContext.Set<ImageRoomType>().Where(s=>s.RoomTypeId == roomTypeId).ToListAsync();
            return imageRoomTypes.ToList();
        }

        public async Task<string> RemoveImageRoomType(UpdateImageRoomType updateImageRoomType)
        {
            var imageRoomTypeResult = await _dbContext.Set<ImageRoomType>().Where(s => s.RoomTypeId == updateImageRoomType.RoomTypeId && s.FileName == updateImageRoomType.FileName).FirstOrDefaultAsync();

            if (imageRoomTypeResult is null)
            {
                return ResponseEnum.SomewhereWrong.ToString();
            }

            _dbContext.Set<ImageRoomType>().Remove(imageRoomTypeResult);

            await _dbContext.SaveChangesAsync();

            return ResponseEnum.Success.ToString();
        }
    }
}
