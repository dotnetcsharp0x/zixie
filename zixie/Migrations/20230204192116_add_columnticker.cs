using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace zixie.Migrations
{
    public partial class add_columnticker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ticker",
                table: "PortfolioItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ticker",
                table: "PortfolioItems");
        }
    }
}
