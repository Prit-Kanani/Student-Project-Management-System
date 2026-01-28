using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectGroup.Migrations;

/// <inheritdoc />
public partial class Adding_UseProfile : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Phone",
            table: "User");

        migrationBuilder.CreateTable(
            name: "UserProfile",
            columns: table => new
            {
                UserProfileID = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserID = table.Column<int>(type: "int", nullable: false),
                FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ProfileImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                Gender = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_UserProfile", x => x.UserProfileID);
                table.ForeignKey(
                    name: "FK_UserProfile_User_UserID",
                    column: x => x.UserID,
                    principalTable: "User",
                    principalColumn: "UserID",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_UserProfile_UserID",
            table: "UserProfile",
            column: "UserID",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "UserProfile");

        migrationBuilder.AddColumn<string>(
            name: "Phone",
            table: "User",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");
    }
}
