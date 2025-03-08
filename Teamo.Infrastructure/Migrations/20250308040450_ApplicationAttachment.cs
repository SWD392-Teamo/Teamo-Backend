using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teamo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentUrl",
                table: "Application",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentUrl",
                table: "Application");
        }
    }
}
