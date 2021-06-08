using Microsoft.EntityFrameworkCore.Migrations;

namespace SquaresApp.Data.Migrations
{
    public partial class fixed_indices_naming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "TDX_User_Username",
                table: "Users",
                newName: "IDX_User_Username");

            migrationBuilder.RenameIndex(
                name: "TDX_Point_UserId_X_Y",
                table: "Points",
                newName: "IDX_Point_UserId_X_Y");

            migrationBuilder.RenameIndex(
                name: "TDX_Point_UserId",
                table: "Points",
                newName: "IDX_Point_UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IDX_User_Username",
                table: "Users",
                newName: "TDX_User_Username");

            migrationBuilder.RenameIndex(
                name: "IDX_Point_UserId_X_Y",
                table: "Points",
                newName: "TDX_Point_UserId_X_Y");

            migrationBuilder.RenameIndex(
                name: "IDX_Point_UserId",
                table: "Points",
                newName: "TDX_Point_UserId");
        }
    }
}
