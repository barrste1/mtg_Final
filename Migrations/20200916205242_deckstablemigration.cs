using Microsoft.EntityFrameworkCore.Migrations;

namespace MagicTheGatheringFinal.Migrations
{
    public partial class deckstablemigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "decksTableKey",
                table: "cardsTable",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSelected",
                table: "cardsTable",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_cardsTable_decksTableKey",
                table: "cardsTable",
                column: "decksTableKey");

            migrationBuilder.AddForeignKey(
                name: "FK__cardsTabl__decks__787EE5A0",
                table: "cardsTable",
                column: "decksTableKey",
                principalTable: "decksTable",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__cardsTabl__decks__787EE5A0",
                table: "cardsTable");

            migrationBuilder.DropIndex(
                name: "IX_cardsTable_decksTableKey",
                table: "cardsTable");

            migrationBuilder.DropColumn(
                name: "decksTableKey",
                table: "cardsTable");

            migrationBuilder.DropColumn(
                name: "IsSelected",
                table: "cardsTable");
        }
    }
}
