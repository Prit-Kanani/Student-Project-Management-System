using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectGroupService.Migrations
{
    /// <inheritdoc />
    public partial class UniqueConInPGID_And_SID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GroupWiseStudent_ProjectGroupID",
                table: "GroupWiseStudent");

            migrationBuilder.CreateIndex(
                name: "IX_GroupWiseStudent_ProjectGroupID_StudentID",
                table: "GroupWiseStudent",
                columns: new[] { "ProjectGroupID", "StudentID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GroupWiseStudent_ProjectGroupID_StudentID",
                table: "GroupWiseStudent");

            migrationBuilder.CreateIndex(
                name: "IX_GroupWiseStudent_ProjectGroupID",
                table: "GroupWiseStudent",
                column: "ProjectGroupID");
        }
    }
}
