using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music_Bot_Telegram.Data.Migrations
{
    public partial class AddIsActiveSessionToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActiveSession",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActiveSession",
                table: "Users");
        }
    }
}
