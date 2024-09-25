using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingServices.Data.Migrations
{
    /// <inheritdoc />
    public partial class solveBookidinServiceAndAllowNullForPaymentIncomeIdinBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_PaymentIncomes_PaymentIncomeId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Packages_Bookings_BookingId",
                table: "Packages");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Bookings_BookingId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_BookingId",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Packages_BookingId",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Packages");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentIncomeId",
                table: "Bookings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_PaymentIncomes_PaymentIncomeId",
                table: "Bookings",
                column: "PaymentIncomeId",
                principalTable: "PaymentIncomes",
                principalColumn: "PaymentIncomeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_PaymentIncomes_PaymentIncomeId",
                table: "Bookings");

            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BookingId",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentIncomeId",
                table: "Bookings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_BookingId",
                table: "Services",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_BookingId",
                table: "Packages",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_PaymentIncomes_PaymentIncomeId",
                table: "Bookings",
                column: "PaymentIncomeId",
                principalTable: "PaymentIncomes",
                principalColumn: "PaymentIncomeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Packages_Bookings_BookingId",
                table: "Packages",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Bookings_BookingId",
                table: "Services",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "BookingId");
        }
    }
}
