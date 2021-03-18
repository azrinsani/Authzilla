using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace authzilla.sqlite.Migrations.PersistedGrantDb
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceCodes",
                columns: table => new
                {
                    usercode = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    devicecode = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    subjectid = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    sessionid = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    clientid = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    creationtime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    expiration = table.Column<DateTime>(type: "TEXT", nullable: false),
                    data = table.Column<string>(type: "TEXT", maxLength: 50000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_devicecodes", x => x.usercode);
                });

            migrationBuilder.CreateTable(
                name: "PersistedGrants",
                columns: table => new
                {
                    key = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    type = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    subjectid = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    sessionid = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    clientid = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    creationtime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    expiration = table.Column<DateTime>(type: "TEXT", nullable: true),
                    consumedtime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    data = table.Column<string>(type: "TEXT", maxLength: 50000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_persistedgrants", x => x.key);
                });

            migrationBuilder.CreateIndex(
                name: "ix_devicecodes_devicecode",
                table: "DeviceCodes",
                column: "devicecode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_devicecodes_expiration",
                table: "DeviceCodes",
                column: "expiration");

            migrationBuilder.CreateIndex(
                name: "ix_persistedgrants_expiration",
                table: "PersistedGrants",
                column: "expiration");

            migrationBuilder.CreateIndex(
                name: "ix_persistedgrants_subjectid_clientid_type",
                table: "PersistedGrants",
                columns: new[] { "subjectid", "clientid", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_persistedgrants_subjectid_sessionid_type",
                table: "PersistedGrants",
                columns: new[] { "subjectid", "sessionid", "type" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceCodes");

            migrationBuilder.DropTable(
                name: "PersistedGrants");
        }
    }
}
