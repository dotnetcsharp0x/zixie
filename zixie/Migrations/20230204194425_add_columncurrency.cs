using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace zixie.Migrations
{
    public partial class add_columncurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "PortfolioItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "PortfolioItems");
        }
    }
}
