using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Infrastructure.Context
{
    public class ApplicationDBContext : IdentityDbContext<User,ApplicationRole,Guid>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ExtraService> ExtraServices { get; set; }
        public DbSet<History> Histories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ImageRoomType> ImageRoomTypes { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().HasMany(s=>s.Histories).WithOne(his => his.Order).HasForeignKey(o=>o.OrderId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Review)
                .WithOne(r => r.Order)
                .HasForeignKey<Order>(o => o.ReviewId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
               .HasOne(r => r.Order)
               .WithOne(o => o.Review)
                .HasForeignKey<Review>(r => r.OrderId).OnDelete(DeleteBehavior.NoAction);

            
            modelBuilder.Entity<ImageRoomType>(imr => imr.HasKey(imr => new { imr.FileName, imr.RoomTypeId }));

            InitDefaultData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void InitDefaultData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("afe55bda-131d-4e62-9691-1c65472f3b55"),
                    FullName = "Admin",
                    FirstName = "Root",
                    LastName = "Admin",
                    Avatar = "NAN",
                    Email = "admin@gmail.com",
                    DateOfBirth = DateTime.Parse("1998-01-08 00:00:00.0000000"),
                    PasswordHash = "AQAAAAEAACcQAAAAEHqtTFXhKmqEfsYvMzUT2qlfx3ZpI4ZQE6xBX1K6VLi5P0sw7bsmVmV/CGf4DxlRXg==",
                    SecurityStamp = "OAXRFOZ2HLMSBS7ZYQD2E236PNMZSAD5",
                    NormalizedEmail = "ADMIN@GMAIL.COM",
                    EmailConfirmed = true,
                    NormalizedUserName = "ADMIN@GMAIL.COM",
                    UserName = "admin@gmail.com",
                    PhoneNumber = "1234567890",
                    Membership = "Gold",
                    Merit = 0,
                    CreateDate = DateTime.Parse("2024-01-08 00:00:00.0000000"),
                }
                );

            modelBuilder.Entity<ApplicationRole>().HasData(
                new ApplicationRole
                {
                    Id = Guid.Parse("b1b6f029-afbb-40a4-9349-15e4d3103aa8"),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "95e5145b-6038-4a57-b290-ef9caf2065aa"
                }, 
                new ApplicationRole
                {
                    Id = Guid.Parse("835b7c90-f612-493d-bf14-14cba95c0fa5"),
                    Name = "Manager",
                    NormalizedName = "MANAGER",
                    ConcurrencyStamp = "63637933-915f-4a1b-9f1c-53214d7715ba"
                },
                new ApplicationRole
                {
                    Id = Guid.Parse("40c36deb-cc1b-47bf-b1a4-03441838fc01"),
                    Name = "Staff",
                    NormalizedName = "STAFF",
                    ConcurrencyStamp = "4debc2d6-3a7f-4890-93eb-de3af9b6e4c6"
                },
                new ApplicationRole
                {
                    Id = Guid.Parse("40dccaf1-fea6-4bb9-ad59-7ca2c4161c16"),
                    Name = "Guest",
                    NormalizedName = "GUEST",
                    ConcurrencyStamp = "8200e584-3a23-4efc-a45d-27137a3236c4"
                }
            );

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("afe55bda-131d-4e62-9691-1c65472f3b55"),
                    RoleId = Guid.Parse("b1b6f029-afbb-40a4-9349-15e4d3103aa8")
                }
            );
        }
    }
}
