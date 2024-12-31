using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Falc.Communications.Infrastructure.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMassTransitOutboxSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "OutboxState",
                newName: "OutboxState",
                newSchema: "communications");

            migrationBuilder.RenameTable(
                name: "OutboxMessage",
                newName: "OutboxMessage",
                newSchema: "communications");

            migrationBuilder.RenameTable(
                name: "InboxState",
                newName: "InboxState",
                newSchema: "communications");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "OutboxState",
                schema: "communications",
                newName: "OutboxState");

            migrationBuilder.RenameTable(
                name: "OutboxMessage",
                schema: "communications",
                newName: "OutboxMessage");

            migrationBuilder.RenameTable(
                name: "InboxState",
                schema: "communications",
                newName: "InboxState");
        }
    }
}
