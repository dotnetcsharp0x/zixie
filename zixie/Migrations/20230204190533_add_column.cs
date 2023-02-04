using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace zixie.Migrations
{
    public partial class add_column : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NameInstrument",
                table: "PortfolioItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NameInstrument",
                table: "PortfolioItems");
        }
    }
}
