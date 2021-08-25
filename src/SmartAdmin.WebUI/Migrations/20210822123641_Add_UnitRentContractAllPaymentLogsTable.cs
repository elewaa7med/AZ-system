using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartAdmin.WebUI.Migrations
{
    public partial class Add_UnitRentContractAllPaymentLogsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "unitRentContractAllPaymentLogs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PaidAmount = table.Column<int>(nullable: false),
                    PaymentDate = table.Column<DateTime>(nullable: false),
                    UnitRentContractPaymentID = table.Column<int>(nullable: false),
                    UserID = table.Column<string>(nullable: true),
                    Action = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unitRentContractAllPaymentLogs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_unitRentContractAllPaymentLogs_TUnitRentContractPayments_UnitRentContractPaymentID",
                        column: x => x.UnitRentContractPaymentID,
                        principalTable: "TUnitRentContractPayments",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_unitRentContractAllPaymentLogs_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_unitRentContractAllPaymentLogs_UnitRentContractPaymentID",
                table: "unitRentContractAllPaymentLogs",
                column: "UnitRentContractPaymentID");

            migrationBuilder.CreateIndex(
                name: "IX_unitRentContractAllPaymentLogs_UserID",
                table: "unitRentContractAllPaymentLogs",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "unitRentContractAllPaymentLogs");
        }
    }
}
