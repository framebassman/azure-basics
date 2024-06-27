using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTasks.Api.Migrations
{
    /// <inheritdoc />
    public partial class Unsyncronized : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "unsyncronized_projects",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unsyncronized_projects", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "unsyncronized_tasks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectReferenceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unsyncronized_tasks", x => x.id);
                    table.ForeignKey(
                        name: "FK_unsyncronized_tasks_unsyncronized_projects_ProjectReferenceId",
                        column: x => x.ProjectReferenceId,
                        principalTable: "unsyncronized_projects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_unsyncronized_tasks_ProjectReferenceId",
                table: "unsyncronized_tasks",
                column: "ProjectReferenceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "unsyncronized_tasks");

            migrationBuilder.DropTable(
                name: "unsyncronized_projects");
        }
    }
}
