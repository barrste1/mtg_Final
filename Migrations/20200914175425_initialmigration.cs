using Microsoft.EntityFrameworkCore.Migrations;

namespace MagicTheGatheringFinal.Migrations
{
    public partial class initialmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cardsTable",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cardsTable", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "quizTable",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    word = table.Column<string>(nullable: true),
                    color = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quizTable", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usersTable",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    playertype = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usersTable", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "decksTable",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cardId = table.Column<int>(nullable: false),
                    UserTableId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_decksTable", x => x.id);
                    table.ForeignKey(
                        name: "FK_decksTable_usersTable_UserTableId",
                        column: x => x.UserTableId,
                        principalTable: "usersTable",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_decksTable_cardsTable_cardId",
                        column: x => x.cardId,
                        principalTable: "cardsTable",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_decksTable_UserTableId",
                table: "decksTable",
                column: "UserTableId");

            migrationBuilder.CreateIndex(
                name: "IX_decksTable_cardId",
                table: "decksTable",
                column: "cardId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "decksTable");

            migrationBuilder.DropTable(
                name: "quizTable");

            migrationBuilder.DropTable(
                name: "usersTable");

            migrationBuilder.DropTable(
                name: "cardsTable");
        }
    }
}
