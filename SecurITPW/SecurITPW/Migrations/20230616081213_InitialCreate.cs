using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecurITPW.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "House",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdHouse = table.Column<int>(type: "int", nullable: false),
                    IdRoom = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_House", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SurName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Access",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPic = table.Column<int>(type: "int", nullable: false),
                    IdUser = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SurName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdHouse = table.Column<int>(type: "int", nullable: false),
                    IdRoom = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HouseId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Access", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Access_House_HouseId",
                        column: x => x.HouseId,
                        principalTable: "House",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Access_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Access_HouseId",
                table: "Access",
                column: "HouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Access_UserId",
                table: "Access",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Access");

            migrationBuilder.DropTable(
                name: "House");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
