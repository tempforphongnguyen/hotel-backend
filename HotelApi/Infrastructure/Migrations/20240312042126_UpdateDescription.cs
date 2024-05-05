using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class UpdateDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Decription",
                table: "Notifications",
                newName: "Description");
                
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("afe55bda-131d-4e62-9691-1c65472f3b55"),
                column: "ConcurrencyStamp",
                value: "8d35a287-bc7e-464d-9e77-06115acb5004");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Notifications",
                newName: "Decription");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("afe55bda-131d-4e62-9691-1c65472f3b55"),
                column: "ConcurrencyStamp",
                value: "712fb3f6-51f5-4234-ad74-90941c4f272c");
        }
    }
}
