using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiapCloudGamesTechChallenge.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Third : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditGameUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GameId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Collection = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateCreated = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditGameUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditGameUser_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuditGameUser_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AuditGameUser_GameId",
                table: "AuditGameUser",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditGameUser_UserId",
                table: "AuditGameUser",
                column: "UserId");            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditGameUser");

            // Remove seeded admin user
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-2222-3333-4444-555555555555")
            );

            // Remove seeded games
            var gameIds = new Guid[]
            {
                new Guid("10101010-1010-1010-1010-101010101010"),
                new Guid("20202020-2020-2020-2020-202020202020"),
                new Guid("30303030-3030-3030-3030-303030303030"),
                new Guid("40404040-4040-4040-4040-404040404040"),
                new Guid("50505050-5050-5050-5050-505050505050"),
                new Guid("60606060-6060-6060-6060-606060606060"),
                new Guid("70707070-7070-7070-7070-707070707070"),
                new Guid("80808080-8080-8080-8080-808080808080"),
                new Guid("90909090-9090-9090-9090-909090909090"),
                new Guid("aaaaaaaa-1111-2222-3333-444444444444")
            };

            foreach (var id in gameIds)
            {
                migrationBuilder.DeleteData(
                    table: "Game",
                    keyColumn: "Id",
                    keyValue: id
                );
            }
        }
    }
}
