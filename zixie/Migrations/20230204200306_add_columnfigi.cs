using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace zixie.Migrations
{
    public partial class add_columnfigi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Figi",
                table: "PortfolioItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Figi",
                table: "PortfolioItems");
        }
    }
}
