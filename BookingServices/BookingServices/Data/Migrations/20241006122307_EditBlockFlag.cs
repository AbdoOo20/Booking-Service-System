using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingServices.Data.Migrations
{
    /// <inheritdoc />
    public partial class EditBlockFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlooked",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "IsBlooked",
                table: "PaymentIncomes");

            migrationBuilder.DropColumn(
                name: "IsBlooked",
                table: "Packages");

            migrationBuilder.RenameColumn(
                name: "IsBlooked",
                table: "Services",
                newName: "IsBlocked");

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "ServiceProviders",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "ProviderContracts",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "PaymentIncomes",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "Packages",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "Consultations",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlocked",
                table: "AdminContracts",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "ProviderContracts");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "PaymentIncomes");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "Consultations");

            migrationBuilder.DropColumn(
                name: "IsBlocked",
                table: "AdminContracts");

            migrationBuilder.RenameColumn(
                name: "IsBlocked",
                table: "Services",
                newName: "IsBlooked");

            migrationBuilder.AddColumn<bool>(
                name: "IsBlooked",
                table: "ServiceProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlooked",
                table: "PaymentIncomes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBlooked",
                table: "Packages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
