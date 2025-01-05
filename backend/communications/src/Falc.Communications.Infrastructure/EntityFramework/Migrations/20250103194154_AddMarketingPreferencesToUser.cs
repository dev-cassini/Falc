using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Falc.Communications.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketingPreferencesToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MarketingPreferences_Email",
                schema: "communications",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MarketingPreferences_Phone",
                schema: "communications",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MarketingPreferences_Sms",
                schema: "communications",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarketingPreferences_Email",
                schema: "communications",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MarketingPreferences_Phone",
                schema: "communications",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MarketingPreferences_Sms",
                schema: "communications",
                table: "Users");
        }
    }
}
