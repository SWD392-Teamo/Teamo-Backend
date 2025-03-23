using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teamo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePrivacy_PostTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Privacy",
                table: "Post");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Privacy",
                table: "Post",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "");
        }
    }
}
