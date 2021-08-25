using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class AddIdMadoob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdMandoob",
                table: "TUnitRentContract",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TUnitRentContract_IdMandoob",
                table: "TUnitRentContract",
                column: "IdMandoob");

            migrationBuilder.AddForeignKey(
                name: "FK_TUnitRentContract_AspNetUsers_IdMandoob",
                table: "TUnitRentContract",
                column: "IdMandoob",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TUnitRentContract_AspNetUsers_IdMandoob",
                table: "TUnitRentContract");

            migrationBuilder.DropIndex(
                name: "IX_TUnitRentContract_IdMandoob",
                table: "TUnitRentContract");

            migrationBuilder.DropColumn(
                name: "IdMandoob",
                table: "TUnitRentContract");
        }
    }
}
