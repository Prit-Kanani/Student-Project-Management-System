using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectGroupService.Migrations
{
    /// <inheritdoc />
    public partial class NameChangedToGWS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentWiseGroup");

            migrationBuilder.CreateTable(
                name: "GroupWiseStudent",
                columns: table => new
                {
                    StudentWiseGroupID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ProjectGroupID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByID = table.Column<int>(type: "int", nullable: false),
                    ModifiedByID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupWiseStudent", x => x.StudentWiseGroupID);
                    table.ForeignKey(
                        name: "FK_GroupWiseStudent_ProjectGroup_ProjectGroupID",
                        column: x => x.ProjectGroupID,
                        principalTable: "ProjectGroup",
                        principalColumn: "ProjectGroupID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupWiseStudent_ProjectGroupID",
                table: "GroupWiseStudent",
                column: "ProjectGroupID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupWiseStudent");

            migrationBuilder.CreateTable(
                name: "StudentWiseGroup",
                columns: table => new
                {
                    StudentWiseGroupID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectGroupID = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByID = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByID = table.Column<int>(type: "int", nullable: true),
                    StudentID = table.Column<int>(type: "int", nullable: false)
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
                name: "IX_StudentWiseGroup_ProjectGroupID",
                table: "StudentWiseGroup",
                column: "ProjectGroupID");
        }
    }
}
