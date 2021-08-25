using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class AddLegalTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LegalId",
                table: "TUnitRentContract",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Legals",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<string>(nullable: true),
                    UpdatedBy = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    IdRentContract = table.Column<int>(nullable: false),
                    RequestNumber = table.Column<string>(nullable: false),
                    RequestDate = table.Column<DateTime>(nullable: false),
                    ElectricityBill = table.Column<int>(nullable: false),
                    WalterBill = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Legals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Legals_TUnitRentContract_IdRentContract",
                        column: x => x.IdRentContract,
                        principalTable: "TUnitRentContract",
                        principalColumn: "IdRentContract",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Legals_IdRentContract",
                table: "Legals",
                column: "IdRentContract",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Legals");

            migrationBuilder.DropColumn(
                name: "LegalId",
                table: "TUnitRentContract");
        }
    }
}
