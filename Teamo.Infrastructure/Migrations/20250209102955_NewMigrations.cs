using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teamo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_AspNetUsers_DestStudentId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_AspNetUsers_SrcStudentId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_DestStudentId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "DestStudentId",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "SrcStudentId",
                table: "Applications",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_SrcStudentId",
                table: "Applications",
                newName: "IX_Applications_StudentId");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AspNetUsers",
                type: "varchar(1000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_AspNetUsers_StudentId",
                table: "Applications",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_AspNetUsers_StudentId",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Applications",
                newName: "SrcStudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Applications_StudentId",
                table: "Applications",
                newName: "IX_Applications_SrcStudentId");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AspNetUsers",
                type: "nvarchar(1000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DestStudentId",
                table: "Applications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_DestStudentId",
                table: "Applications",
                column: "DestStudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_AspNetUsers_DestStudentId",
                table: "Applications",
                column: "DestStudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_AspNetUsers_SrcStudentId",
                table: "Applications",
                column: "SrcStudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
