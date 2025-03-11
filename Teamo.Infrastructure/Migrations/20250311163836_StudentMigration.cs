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
                name: "Major",
                table: "Student",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Student_Major",
                table: "Student",
                column: "Major",
                unique: true,
                filter: "[Major] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Student_Major",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "Major",
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
