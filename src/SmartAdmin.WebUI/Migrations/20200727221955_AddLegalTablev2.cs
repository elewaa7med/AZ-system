using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class AddLegalTablev2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RequestDate",
                table: "Legals",
                newName: "RequestSubmitDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestRaiseDate",
                table: "Legals",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestRaiseDate",
                table: "Legals");

            migrationBuilder.RenameColumn(
                name: "RequestSubmitDate",
                table: "Legals",
                newName: "RequestDate");
        }
    }
}
