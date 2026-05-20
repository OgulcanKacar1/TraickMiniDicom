using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraickMiniDicom.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthAndDataIsolation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "Role");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Studies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Studies_UserId",
                table: "Studies",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Studies_Users_UserId",
                table: "Studies",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Studies_Users_UserId",
                table: "Studies");

            migrationBuilder.DropIndex(
                name: "IX_Studies_UserId",
                table: "Studies");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Studies");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Users",
                newName: "Username");
        }
    }
}
