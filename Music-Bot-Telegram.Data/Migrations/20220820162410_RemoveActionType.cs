using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Music_Bot_Telegram.Data.Migrations
{
    public partial class RemoveActionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_ActionTypes_TypeId",
                table: "Actions");

            migrationBuilder.DropTable(
                name: "ActionTypes");

            migrationBuilder.DropIndex(
                name: "IX_Actions_TypeId",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Actions");

            migrationBuilder.AddColumn<string>(
                name: "Command",
                table: "Actions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Command",
                table: "Actions");

            migrationBuilder.AddColumn<long>(
                name: "TypeId",
                table: "Actions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "ActionTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actions_TypeId",
                table: "Actions",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionTypes_Name",
                table: "ActionTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_ActionTypes_TypeId",
                table: "Actions",
                column: "TypeId",
                principalTable: "ActionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
