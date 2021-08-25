using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class ElectricityMeter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ElectricityMeters",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ElectricityMeterNumber = table.Column<string>(nullable: true),
                    PaymentNumber = table.Column<string>(nullable: true),
                    StartOfMeter = table.Column<string>(nullable: true),
                    TransferTheAccountToTenant = table.Column<bool>(nullable: false),
                    UnitID = table.Column<int>(nullable: true),
                    CompoundUnitID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElectricityMeters", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ElectricityMeters_TCompoundUnits_CompoundUnitID",
                        column: x => x.CompoundUnitID,
                        principalTable: "TCompoundUnits",
                        principalColumn: "IdUnit",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ElectricityMeters_Units_UnitID",
                        column: x => x.UnitID,
                        principalTable: "Units",
                        principalColumn: "IdUnit",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElectricityMeters_CompoundUnitID",
                table: "ElectricityMeters",
                column: "CompoundUnitID");

            migrationBuilder.CreateIndex(
                name: "IX_ElectricityMeters_UnitID",
                table: "ElectricityMeters",
                column: "UnitID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElectricityMeters");
        }
    }
}
