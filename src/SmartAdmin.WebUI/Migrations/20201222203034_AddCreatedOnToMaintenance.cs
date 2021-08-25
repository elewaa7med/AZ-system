using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class AddCreatedOnToMaintenance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockReason",
                table: "TUnitRentContract",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Blocked",
                table: "TUnitRentContract",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Maintenances",
                nullable: false,
                defaultValue: DateTime.Now);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockReason",
                table: "TUnitRentContract");

            migrationBuilder.DropColumn(
                name: "Blocked",
                table: "TUnitRentContract");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Maintenances");
        }
    }
}
