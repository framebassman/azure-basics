using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTasks.DataAccess.AzureSQL.Migrations
{
    /// <inheritdoc />
    public partial class WasSyncronized : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WasSynchronized",
                table: "tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WasSynchronized",
                table: "projects",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WasSynchronized",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "WasSynchronized",
                table: "projects");
        }
    }
}
