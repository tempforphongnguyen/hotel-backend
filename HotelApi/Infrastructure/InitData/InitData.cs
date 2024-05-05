using Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.InitData
{
    public class InitData
    {
        public void Init()
        {
            var initDepartment = new List<Department>()
            {
                new Department()
                {
                    Id = Guid.Parse("ef5e2717-a54c-4673-8af0-2603d721cce0"),
                    Location = "Location 1",
                    Name = "Location Name 1",
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    IsActive = true,
                    LocationLink = "localtion1.com",
                    PhoneNumber = "1234567890",
                    Status = "Active",
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                },
                new Department()
                {
                    Id = Guid.Parse("ef5e2717-a54c-4673-8af0-2603d721cce1"),
                    Location = "Location 2",
                    Name = "Location Name 2",
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    IsActive = true,
                    LocationLink = "localtion2.com",
                    PhoneNumber = "1234567890",
                    Status = "Active",
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                },
                new Department()
                {
                    Id = Guid.Parse("ef5e2717-a54c-4673-8af0-2603d721cce2"),
                    Location = "Location 3",
                    Name = "Location Name 3",
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    IsActive = true,
                    LocationLink = "localtion3.com",
                    PhoneNumber = "1234567890",
                    Status = "Active",
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                },
                new Department()
                {
                    Id = Guid.Parse("ef5e2717-a54c-4673-8af0-2603d721cce3"),
                    Location = "Location 4",
                    Name = "Location Name 4",
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    IsActive = true,
                    LocationLink = "localtion4.com",
                    PhoneNumber = "1234567890",
                    Status = "Active",
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                },
                new Department()
                {
                    Id = Guid.Parse("ef5e2717-a54c-4673-8af0-2603d721cce4"),
                    Location = "Location 5",
                    Name = "Location Name 5",
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    IsActive = true,
                    LocationLink = "localtion5.com",
                    PhoneNumber = "1234567890",
                    Status = "Active",
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                },
                new Department()
                {
                    Id = Guid.Parse("ef5e2717-a54c-4673-8af0-2603d721cce5"),
                    Location = "Location 6",
                    Name = "Location Name 6",
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    IsActive = true,
                    LocationLink = "localtion1.com",
                    PhoneNumber = "1234567890",
                    Status = "Active",
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                }
            };

            var initRoomType = new List<RoomType>()
            {
                new RoomType()
                {
                    Id = Guid.Parse("dce37995-c788-4836-91be-07458c8a80a1"),
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    View = "Sea",
                    Description = "Description",
                    IsActive = true,
                    MaxChild = 1,
                    MaxPerson = 2,
                    Name = "RoomType 1",
                    Price = 50,
                    Status = "Active",
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376")
                },
                new RoomType()
                {
                    Id = Guid.Parse("dce37995-c788-4836-91be-07458c8a80a2"),
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    View = "Sea",
                    Description = "Description",
                    IsActive = true,
                    MaxChild = 1,
                    MaxPerson = 2,
                    Name = "RoomType 2",
                    Price = 50,
                    Status = "Active",
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376")
                },
                new RoomType()
                {
                    Id = Guid.Parse("dce37995-c788-4836-91be-07458c8a80a3"),
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    View = "Mountain",
                    Description = "Description",
                    IsActive = true,
                    MaxChild = 1,
                    MaxPerson = 2,
                    Name = "RoomType 3",
                    Price = 150,
                    Status = "Active",
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376")
                },
                new RoomType()
                {
                    Id = Guid.Parse("dce37995-c788-4836-91be-07458c8a80a4"),
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    View = "Mountain",
                    Description = "Description",
                    IsActive = true,
                    MaxChild = 2,
                    MaxPerson = 4,
                    Name = "RoomType 4",
                    Price = 250,
                    Status = "Active",
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376")
                },
                new RoomType()
                {
                    Id = Guid.Parse("dce37995-c788-4836-91be-07458c8a80a5"),
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    View = "Sea",
                    Description = "Description",
                    IsActive = true,
                    MaxChild = 2,
                    MaxPerson = 4,
                    Name = "RoomType 5",
                    Price = 250,
                    Status = "Active",
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376")
                },
                new RoomType()
                {
                    Id = Guid.Parse("dce37995-c788-4836-91be-07458c8a80a6"),
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    View = "Sea",
                    Description = "Description",
                    IsActive = true,
                    MaxChild = 2,
                    MaxPerson = 6,
                    Name = "RoomType 6",
                    Price = 450,
                    Status = "Active",
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376")
                }
            };

            var initRoom = new List<Room>()
            {
                new Room()
                {
                    Id = Guid.Parse("92f59e88-9704-4f1e-8dc2-7a8627f1cee1"),
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    Status = "Active",
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    Name = "Room 01",
                    IsActive = true,
                    RoomTypeId = Guid.Parse("dce37995-c788-4836-91be-07458c8a80a6"),
                    DepartmentId = Guid.Parse("ef5e2717-a54c-4673-8af0-2603d721cce0")
                },
                new Room()
                {
                    Id = Guid.Parse("92f59e88-9704-4f1e-8dc2-7a8627f1cee2"),
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    Status = "Active",
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    Name = "Room 02",
                    IsActive = true,
                    RoomTypeId = Guid.Parse("dce37995-c788-4836-91be-07458c8a80a6"),
                    DepartmentId = Guid.Parse("ef5e2717-a54c-4673-8af0-2603d721cce0")
                },
                new Room()
                {
                    Id = Guid.Parse("92f59e88-9704-4f1e-8dc2-7a8627f1cee3"),
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    Status = "Active",
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    Name = "Room 03",
                    IsActive = true,
                    RoomTypeId = Guid.Parse("dce37995-c788-4836-91be-07458c8a80a2"),
                    DepartmentId = Guid.Parse("ef5e2717-a54c-4673-8af0-2603d721cce0")
                },
                new Room()
                {
                    Id = Guid.Parse("92f59e88-9704-4f1e-8dc2-7a8627f1cee4"),
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    Status = "Active",
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    Name = "Room 04",
                    IsActive = true,
                    RoomTypeId = Guid.Parse("dce37995-c788-4836-91be-07458c8a80a6"),
                    DepartmentId = Guid.Parse("ef5e2717-a54c-4673-8af0-2603d721cce0")
                },
                new Room()
                {
                    Id = Guid.Parse("92f59e88-9704-4f1e-8dc2-7a8627f1cee5"),
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    Status = "Active",
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    Name = "Room 05",
                    IsActive = true,
                    RoomTypeId = Guid.Parse("dce37995-c788-4836-91be-07458c8a80a4"),
                    DepartmentId = Guid.Parse("ef5e2717-a54c-4673-8af0-2603d721cce1")
                },
                new Room()
                {
                    Id = Guid.Parse("92f59e88-9704-4f1e-8dc2-7a8627f1cee6"),
                    UpdateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    Status = "Active",
                    CreateDate = DateTime.Parse("2024-02-22 18:30:17.7905376"),
                    Name = "Room 06",
                    IsActive = true,
                    RoomTypeId = Guid.Parse("dce37995-c788-4836-91be-07458c8a80a4"),
                    DepartmentId = Guid.Parse("ef5e2717-a54c-4673-8af0-2603d721cce1")
                }
            };
        }
    }
}
