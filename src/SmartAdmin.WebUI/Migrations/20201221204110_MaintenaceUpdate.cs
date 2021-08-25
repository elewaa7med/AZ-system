using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class MaintenaceUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Carpentry",
                table: "Maintenances",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CarpentryDesc",
                table: "Maintenances",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Conditioning",
                table: "Maintenances",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ConditioningDesc",
                table: "Maintenances",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Electricity",
                table: "Maintenances",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ElectricityDesc",
                table: "Maintenances",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNo",
                table: "Maintenances",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Kitchen",
                table: "Maintenances",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "KitchenDesc",
                table: "Maintenances",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Others",
                table: "Maintenances",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "OthersDesc",
                table: "Maintenances",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Paint",
                table: "Maintenances",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PaintDesc",
                table: "Maintenances",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Plumbing",
                table: "Maintenances",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PlumbingDesc",
                table: "Maintenances",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Tiles",
                table: "Maintenances",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TilesDesc",
                table: "Maintenances",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Toilet",
                table: "Maintenances",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ToiletDesc",
                table: "Maintenances",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "Maintenances",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Waste",
                table: "Maintenances",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "WasteDesc",
                table: "Maintenances",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WaterHeater",
                table: "Maintenances",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "WaterHeaterDesc",
                table: "Maintenances",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Carpentry",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "CarpentryDesc",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Conditioning",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "ConditioningDesc",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Electricity",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "ElectricityDesc",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "InvoiceNo",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Kitchen",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "KitchenDesc",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Others",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "OthersDesc",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Paint",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "PaintDesc",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Plumbing",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "PlumbingDesc",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Tiles",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "TilesDesc",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Toilet",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "ToiletDesc",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Waste",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "WasteDesc",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "WaterHeater",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "WaterHeaterDesc",
                table: "Maintenances");
        }
    }
}
