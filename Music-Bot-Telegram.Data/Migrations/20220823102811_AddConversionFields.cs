using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Music_Bot_Telegram.Data.Migrations
{
    public partial class AddConversionFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConversionId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActiveConversion",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConversionId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsActiveConversion",
                table: "Users");
        }
    }
}
