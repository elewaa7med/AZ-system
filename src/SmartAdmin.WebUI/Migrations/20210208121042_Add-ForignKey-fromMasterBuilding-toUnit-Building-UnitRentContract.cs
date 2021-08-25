using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class AddForignKeyfromMasterBuildingtoUnitBuildingUnitRentContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MasterBuildingsIdMBuilding",
                table: "Units",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "idMasterBuilding",
                table: "Units",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MasterBuildingsIdMBuilding",
                table: "TUnitRentContract",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "mMasterBuilding",
                table: "TUnitRentContract",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MasterBuildingsIdMBuilding",
                table: "TBuildings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "idMasterBuilding",
                table: "TBuildings",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MasterBuildings",
                columns: table => new
                {
                    IdMBuilding = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MBuildingName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterBuildings", x => x.IdMBuilding);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Units_MasterBuildingsIdMBuilding",
                table: "Units",
                column: "MasterBuildingsIdMBuilding");

            migrationBuilder.CreateIndex(
                name: "IX_TUnitRentContract_MasterBuildingsIdMBuilding",
                table: "TUnitRentContract",
                column: "MasterBuildingsIdMBuilding");

            migrationBuilder.CreateIndex(
                name: "IX_TBuildings_MasterBuildingsIdMBuilding",
                table: "TBuildings",
                column: "MasterBuildingsIdMBuilding");

            migrationBuilder.AddForeignKey(
                name: "FK_TBuildings_MasterBuildings_MasterBuildingsIdMBuilding",
                table: "TBuildings",
                column: "MasterBuildingsIdMBuilding",
                principalTable: "MasterBuildings",
                principalColumn: "IdMBuilding",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TUnitRentContract_MasterBuildings_MasterBuildingsIdMBuilding",
                table: "TUnitRentContract",
                column: "MasterBuildingsIdMBuilding",
                principalTable: "MasterBuildings",
                principalColumn: "IdMBuilding",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Units_MasterBuildings_MasterBuildingsIdMBuilding",
                table: "Units",
                column: "MasterBuildingsIdMBuilding",
                principalTable: "MasterBuildings",
                principalColumn: "IdMBuilding",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TBuildings_MasterBuildings_MasterBuildingsIdMBuilding",
                table: "TBuildings");

            migrationBuilder.DropForeignKey(
                name: "FK_TUnitRentContract_MasterBuildings_MasterBuildingsIdMBuilding",
                table: "TUnitRentContract");

            migrationBuilder.DropForeignKey(
                name: "FK_Units_MasterBuildings_MasterBuildingsIdMBuilding",
                table: "Units");

            migrationBuilder.DropTable(
                name: "MasterBuildings");

            migrationBuilder.DropIndex(
                name: "IX_Units_MasterBuildingsIdMBuilding",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_TUnitRentContract_MasterBuildingsIdMBuilding",
                table: "TUnitRentContract");

            migrationBuilder.DropIndex(
                name: "IX_TBuildings_MasterBuildingsIdMBuilding",
                table: "TBuildings");

            migrationBuilder.DropColumn(
                name: "MasterBuildingsIdMBuilding",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "idMasterBuilding",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "MasterBuildingsIdMBuilding",
                table: "TUnitRentContract");

            migrationBuilder.DropColumn(
                name: "mMasterBuilding",
                table: "TUnitRentContract");

            migrationBuilder.DropColumn(
                name: "MasterBuildingsIdMBuilding",
                table: "TBuildings");

            migrationBuilder.DropColumn(
                name: "idMasterBuilding",
                table: "TBuildings");
        }
    }
}
