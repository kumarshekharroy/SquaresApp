using Microsoft.EntityFrameworkCore.Migrations;

namespace SquaresApp.Data.Migrations
{
    public partial class added_point_configuration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Points",
                table: "Points");

            migrationBuilder.RenameIndex(
                name: "IX_Points_UserId",
                table: "Points",
                newName: "TDX_Point_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Points_Id",
                table: "Points",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Points",
                columns: new[] { "Id", "UserId", "X", "Y" },
                values: new object[,]
                {
                    { 1L, 1L, -1, 1 },
                    { 2L, 1L, 1, -1 },
                    { 3L, 1L, 1, 1 },
                    { 4L, 1L, -1, -1 },
                    { 5L, 2L, -1, 1 },
                    { 6L, 2L, 1, -1 },
                    { 7L, 2L, 1, 1 },
                    { 8L, 2L, -1, -1 }
                });

            migrationBuilder.CreateIndex(
                name: "TDX_Point_UserId_X_Y",
                table: "Points",
                columns: new[] { "UserId", "X", "Y" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Points_Id",
                table: "Points");

            migrationBuilder.DropIndex(
                name: "TDX_Point_UserId_X_Y",
                table: "Points");

            migrationBuilder.DeleteData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "Points",
                keyColumn: "Id",
                keyValue: 8L);

            migrationBuilder.RenameIndex(
                name: "TDX_Point_UserId",
                table: "Points",
                newName: "IX_Points_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Points",
                table: "Points",
                column: "Id");
        }
    }
}
