using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ViewModel.RoomType
{
    public class CreateUpdateRoomTypeVM
    {
        public Entities.RoomType RoomType { get; set; }
        public IList<ImageRoomTypeCreateUpdate> ImageRoomTypes { get; set; }
    }
    public class ImageRoomTypeCreateUpdate
    {
        public string ImageRoomTypeName { get; set; }
        public bool IsCreating { get; set; }
    }
}
