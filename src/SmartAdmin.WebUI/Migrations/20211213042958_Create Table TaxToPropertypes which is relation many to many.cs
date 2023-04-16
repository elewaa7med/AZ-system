using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class CreateTableTaxToPropertypeswhichisrelationmanytomany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "taxToProperttypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TaxRateId = table.Column<int>(nullable: false),
                    PropertyTypesId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taxToProperttypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_taxToProperttypes_TPropertyTypes_PropertyTypesId",
                        column: x => x.PropertyTypesId,
                        principalTable: "TPropertyTypes",
                        principalColumn: "IdPropertyType",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_taxToProperttypes_TaxRate_TaxRateId",
                        column: x => x.TaxRateId,
                        principalTable: "TaxRate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_taxToProperttypes_PropertyTypesId",
                table: "taxToProperttypes",
                column: "PropertyTypesId");

            migrationBuilder.CreateIndex(
                name: "IX_taxToProperttypes_TaxRateId",
                table: "taxToProperttypes",
                column: "TaxRateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "taxToProperttypes");
        }
    }
}
