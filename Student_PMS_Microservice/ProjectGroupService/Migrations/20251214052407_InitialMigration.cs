using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectGroupService.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectGroup",
                columns: table => new
                {
                    ProjectGroupID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectGroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ApprovedByID = table.Column<int>(type: "int", nullable: true),
                    CreatedByID = table.Column<int>(type: "int", nullable: false),
                    ModifiedByID = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectGroup", x => x.ProjectGroupID);
                });

            migrationBuilder.CreateTable(
                name: "ProjectGroupByProject",
                columns: table => new
                {
                    ProjectGroupByProjectID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProjectGroupID = table.Column<int>(type: "int", nullable: false),
                    ProjectID = table.Column<int>(type: "int", nullable: false),
                    CreatedByID = table.Column<int>(type: "int", nullable: false),
                    ModifiedByID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectGroupByProject", x => x.ProjectGroupByProjectID);
                    table.ForeignKey(
                        name: "FK_ProjectGroupByProject_ProjectGroup_ProjectGroupID",
                        column: x => x.ProjectGroupID,
                        principalTable: "ProjectGroup",
                        principalColumn: "ProjectGroupID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "StudentWiseGroup",
                columns: table => new
                {
                    StudentWiseGroupID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProjectGroupID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    CreatedByID = table.Column<int>(type: "int", nullable: false),
                    ModifiedByID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentWiseGroup", x => x.StudentWiseGroupID);
                    table.ForeignKey(
                        name: "FK_StudentWiseGroup_ProjectGroup_ProjectGroupID",
                        column: x => x.ProjectGroupID,
                        principalTable: "ProjectGroup",
                        principalColumn: "ProjectGroupID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectGroupByProject_ProjectGroupID",
                table: "ProjectGroupByProject",
                column: "ProjectGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWiseGroup_ProjectGroupID",
                table: "StudentWiseGroup",
                column: "ProjectGroupID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectGroupByProject");

            migrationBuilder.DropTable(
                name: "StudentWiseGroup");

            migrationBuilder.DropTable(
                name: "ProjectGroup");
        }
    }
}
