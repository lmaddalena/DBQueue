using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBQueue.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageBodies",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageBodies", x => x.MessageId);
                });

            migrationBuilder.CreateTable(
                name: "MessageHeaders",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EnqueuedDT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DequeuedDT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockDT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    Version = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageHeaders", x => x.MessageId);
                });

            migrationBuilder.CreateTable(
                name: "MessageJournals",
                columns: table => new
                {
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageJournals", x => x.MessageId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageBodies");

            migrationBuilder.DropTable(
                name: "MessageHeaders");

            migrationBuilder.DropTable(
                name: "MessageJournals");
        }
    }
}
