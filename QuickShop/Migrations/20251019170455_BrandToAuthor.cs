using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickShop.Migrations
{
    /// <inheritdoc />
    public partial class BrandToAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Brand",
                table: "Products",
                newName: "Author");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Author",
                table: "Products",
                newName: "Brand");
        }
    }
}
