using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeniaLite.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSymptomLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SymptomLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Severity0to10 = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    LoggedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SymptomLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TriggerTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriggerTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SymptomLogId = table.Column<Guid>(type: "uuid", nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_SymptomLogs_SymptomLogId",
                        column: x => x.SymptomLogId,
                        principalTable: "SymptomLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SymptomLogsTrigger",
                columns: table => new
                {
                    SymptomLogId = table.Column<Guid>(type: "uuid", nullable: false),
                    TriggerTagId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SymptomLogsTrigger", x => new { x.SymptomLogId, x.TriggerTagId });
                    table.ForeignKey(
                        name: "FK_SymptomLogsTrigger_SymptomLogs_SymptomLogId",
                        column: x => x.SymptomLogId,
                        principalTable: "SymptomLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SymptomLogsTrigger_TriggerTags_TriggerTagId",
                        column: x => x.TriggerTagId,
                        principalTable: "TriggerTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_SymptomLogId",
                table: "Photos",
                column: "SymptomLogId");

            migrationBuilder.CreateIndex(
                name: "IX_Photos_UserId_CreatedAtUtc",
                table: "Photos",
                columns: new[] { "UserId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SymptomLogs_UserId_LoggedAtUtc",
                table: "SymptomLogs",
                columns: new[] { "UserId", "LoggedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SymptomLogsTrigger_TriggerTagId",
                table: "SymptomLogsTrigger",
                column: "TriggerTagId");

            migrationBuilder.CreateIndex(
                name: "IX_TriggerTags_UserId_Name",
                table: "TriggerTags",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "SymptomLogsTrigger");

            migrationBuilder.DropTable(
                name: "SymptomLogs");

            migrationBuilder.DropTable(
                name: "TriggerTags");
        }
    }
}
