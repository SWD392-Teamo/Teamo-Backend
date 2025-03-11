using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teamo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StudentMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MajorId",
                table: "Student");

            migrationBuilder.AddColumn<string>(
                name: "MajorCode",
                table: "Student",
                type: "varchar(20)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MajorCode",
                table: "Student");

            migrationBuilder.AddColumn<int>(
                name: "MajorId",
                table: "Student",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
