using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurITPW.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Access_House_HouseId",
                table: "Access");

            migrationBuilder.DropForeignKey(
                name: "FK_Access_User_UserId",
                table: "Access");

            migrationBuilder.DropIndex(
                name: "IX_Access_HouseId",
                table: "Access");

            migrationBuilder.DropIndex(
                name: "IX_Access_UserId",
                table: "Access");

            migrationBuilder.DropColumn(
                name: "HouseId",
                table: "Access");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Access");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HouseId",
                table: "Access",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Access",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Access_HouseId",
                table: "Access",
                column: "HouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Access_UserId",
                table: "Access",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Access_House_HouseId",
                table: "Access",
                column: "HouseId",
                principalTable: "House",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Access_User_UserId",
                table: "Access",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
