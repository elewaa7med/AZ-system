using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class createtableUnitRentContractOtherPayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "unitRentContractOtherPayment",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Commession = table.Column<int>(nullable: false),
                    Insurence = table.Column<int>(nullable: false),
                    OtherPayment = table.Column<int>(nullable: false),
                    PaidAmount = table.Column<int>(nullable: false),
                    Note = table.Column<string>(nullable: true),
                    PaymentDate = table.Column<DateTime>(nullable: true),
                    MonyType = table.Column<int>(nullable: false),
                    UnitRentContractID = table.Column<int>(nullable: false),
                    UserID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unitRentContractOtherPayment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_unitRentContractOtherPayment_TUnitRentContract_UnitRentContractID",
                        column: x => x.UnitRentContractID,
                        principalTable: "TUnitRentContract",
                        principalColumn: "IdRentContract",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_unitRentContractOtherPayment_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_unitRentContractOtherPayment_UnitRentContractID",
                table: "unitRentContractOtherPayment",
                column: "UnitRentContractID");

            migrationBuilder.CreateIndex(
                name: "IX_unitRentContractOtherPayment_UserID",
                table: "unitRentContractOtherPayment",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "unitRentContractOtherPayment");
        }
    }
}
