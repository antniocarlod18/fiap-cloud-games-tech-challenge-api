using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiapCloudGamesTechChallenge.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Firth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditGameUser");

            migrationBuilder.DropTable(
                name: "GameOrder");

            migrationBuilder.CreateTable(
                name: "AuditGamePrice",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GameId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OldPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    NewPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Justification = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateCreated = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditGamePrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditGamePrice_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AuditGameUserCollection",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GameId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Collection = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateCreated = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditGameUserCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditGameUserCollection_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuditGameUserCollection_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OrderGameItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GameId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderGameItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderGameItem_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderGameItem_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AuditGamePrice_GameId",
                table: "AuditGamePrice",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditGameUserCollection_GameId",
                table: "AuditGameUserCollection",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditGameUserCollection_UserId",
                table: "AuditGameUserCollection",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGameItem_GameId",
                table: "OrderGameItem",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderGameItem_OrderId",
                table: "OrderGameItem",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditGamePrice");

            migrationBuilder.DropTable(
                name: "AuditGameUserCollection");

            migrationBuilder.DropTable(
                name: "OrderGameItem");

            migrationBuilder.CreateTable(
                name: "AuditGameUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    GameId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Collection = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    Notes = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
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

            migrationBuilder.CreateTable(
                name: "GameOrder",
                columns: table => new
                {
                    GamesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameOrder", x => new { x.GamesId, x.OrderId });
                    table.ForeignKey(
                        name: "FK_GameOrder_Game_GamesId",
                        column: x => x.GamesId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameOrder_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
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

            migrationBuilder.CreateIndex(
                name: "IX_GameOrder_OrderId",
                table: "GameOrder",
                column: "OrderId");
        }
    }
}
