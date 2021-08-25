using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class NotesEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "TUnitRentContractNotes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    UnitRentContractID = table.Column<int>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    UserID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TUnitRentContractNotes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TUnitRentContractNotes_TUnitRentContract_UnitRentContractID",
                        column: x => x.UnitRentContractID,
                        principalTable: "TUnitRentContract",
                        principalColumn: "IdRentContract",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TUnitRentContractNotes_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TUnitRentContractNotes_UnitRentContractID",
                table: "TUnitRentContractNotes",
                column: "UnitRentContractID");

            migrationBuilder.CreateIndex(
                name: "IX_TUnitRentContractNotes_UserID",
                table: "TUnitRentContractNotes",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TUnitRentContractNotes");
        }
    }
}
