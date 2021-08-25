﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class H1Renewal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Archived",
                table: "TUnitRentContract",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Renewed",
                table: "TUnitRentContract",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Archived",
                table: "TUnitRentContract");

            migrationBuilder.DropColumn(
                name: "Renewed",
                table: "TUnitRentContract");
        }
    }
}
