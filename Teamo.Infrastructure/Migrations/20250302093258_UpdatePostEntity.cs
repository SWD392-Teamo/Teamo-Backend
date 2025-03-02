using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teamo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePostEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_GroupMember_GroupMemberId",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "GroupMemberId",
                table: "Post",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_GroupMemberId",
                table: "Post",
                newName: "IX_Post_StudentId");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Post",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Post_GroupId",
                table: "Post",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Group_GroupId",
                table: "Post",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Student_StudentId",
                table: "Post",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Post_Group_GroupId",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Student_StudentId",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_GroupId",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Post");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Post",
                newName: "GroupMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_Post_StudentId",
                table: "Post",
                newName: "IX_Post_GroupMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_GroupMember_GroupMemberId",
                table: "Post",
                column: "GroupMemberId",
                principalTable: "GroupMember",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
