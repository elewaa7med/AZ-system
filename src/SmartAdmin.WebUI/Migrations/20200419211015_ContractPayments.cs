using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class ContractPayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TUnitRentContractPayments",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<int>(nullable: false),
                    RemainingAmount = table.Column<int>(nullable: false),
                    Paid = table.Column<bool>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    UnitRentContractID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TUnitRentContractPayments", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TUnitRentContractPayments_TUnitRentContract_UnitRentContractID",
                        column: x => x.UnitRentContractID,
                        principalTable: "TUnitRentContract",
                        principalColumn: "IdRentContract",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TUnitRentContractPayments_UnitRentContractID",
                table: "TUnitRentContractPayments",
                column: "UnitRentContractID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TUnitRentContractPayments");
        }
    }
}
