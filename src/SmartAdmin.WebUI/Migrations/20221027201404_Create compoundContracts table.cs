using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class CreatecompoundContractstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TCompoundUnits_UnitRentContractID",
                table: "TCompoundUnits");

            migrationBuilder.CreateTable(
                name: "compoundContracts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    dtLeaseStart = table.Column<DateTime>(nullable: false),
                    dtLeaseEnd = table.Column<DateTime>(nullable: false),
                    IdCreated = table.Column<string>(nullable: true),
                    dtCreated = table.Column<DateTime>(nullable: false),
                    IdModified = table.Column<string>(nullable: true),
                    dtModified = table.Column<DateTime>(nullable: false),
                    contractImage = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compoundContracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_compoundContracts_AspNetUsers_IdCreated",
                        column: x => x.IdCreated,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TCompoundUnits_UnitRentContractID",
                table: "TCompoundUnits",
                column: "UnitRentContractID",
                unique: true,
                filter: "[UnitRentContractID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_compoundContracts_IdCreated",
                table: "compoundContracts",
                column: "IdCreated");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "compoundContracts");

            migrationBuilder.DropIndex(
                name: "IX_TCompoundUnits_UnitRentContractID",
                table: "TCompoundUnits");

            migrationBuilder.CreateIndex(
                name: "IX_TCompoundUnits_UnitRentContractID",
                table: "TCompoundUnits",
                column: "UnitRentContractID");
        }
    }
}
