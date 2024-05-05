using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class UpdateMaxPerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxPerson",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("afe55bda-131d-4e62-9691-1c65472f3b55"),
                column: "ConcurrencyStamp",
                value: "712fb3f6-51f5-4234-ad74-90941c4f272c");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxPerson",
                table: "OrderDetails");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("afe55bda-131d-4e62-9691-1c65472f3b55"),
                column: "ConcurrencyStamp",
                value: "8699df98-9d2d-4795-8afc-6b62f032b7b6");
        }
    }
}
