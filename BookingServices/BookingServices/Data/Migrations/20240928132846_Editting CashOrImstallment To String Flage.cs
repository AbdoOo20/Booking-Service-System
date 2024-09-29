using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingServices.Data.Migrations
{
    /// <inheritdoc />
    public partial class EdittingCashOrImstallmentToStringFlage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CashOrInstallment",
                table: "Bookings");

            migrationBuilder.AddColumn<string>(
                name: "CashOrCashByHandOrInstallment",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CashOrCashByHandOrInstallment",
                table: "Bookings");

            migrationBuilder.AddColumn<bool>(
                name: "CashOrInstallment",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
