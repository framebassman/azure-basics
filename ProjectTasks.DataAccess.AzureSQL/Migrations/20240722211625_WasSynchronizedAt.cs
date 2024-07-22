using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTasks.DataAccess.AzureSQL.Migrations
{
    /// <inheritdoc />
    public partial class WasSynchronizedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "WasSynchronizedAt",
                table: "tasks",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "WasSynchronizedAt",
                table: "projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WasSynchronizedAt",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "WasSynchronizedAt",
                table: "projects");
        }
    }
}
