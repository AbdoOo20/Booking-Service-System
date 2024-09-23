using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingServices.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNameAndIsOnlineOrOfflineCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ServiceProviders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsOnlineOrOfflineUser",
                table: "Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ServiceProviders");

            migrationBuilder.DropColumn(
                name: "IsOnlineOrOfflineUser",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Customers");
        }
    }
}
