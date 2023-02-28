using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kipaplus.Data.Migrations
{
    /// <inheritdoc />
    public partial class tehtvastuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TäyttäjäUserId",
                table: "TehtavaVastaus",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TarkistajaUserId",
                table: "TehtavaVastaus",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JatkajaUserId",
                table: "TehtavaVastaus",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TehtavaVastaus_JatkajaUserId",
                table: "TehtavaVastaus",
                column: "JatkajaUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TehtavaVastaus_TarkistajaUserId",
                table: "TehtavaVastaus",
                column: "TarkistajaUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TehtavaVastaus_TäyttäjäUserId",
                table: "TehtavaVastaus",
                column: "TäyttäjäUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TehtavaVastaus_AspNetUsers_JatkajaUserId",
                table: "TehtavaVastaus",
                column: "JatkajaUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TehtavaVastaus_AspNetUsers_TarkistajaUserId",
                table: "TehtavaVastaus",
                column: "TarkistajaUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TehtavaVastaus_AspNetUsers_TäyttäjäUserId",
                table: "TehtavaVastaus",
                column: "TäyttäjäUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TehtavaVastaus_AspNetUsers_JatkajaUserId",
                table: "TehtavaVastaus");

            migrationBuilder.DropForeignKey(
                name: "FK_TehtavaVastaus_AspNetUsers_TarkistajaUserId",
                table: "TehtavaVastaus");

            migrationBuilder.DropForeignKey(
                name: "FK_TehtavaVastaus_AspNetUsers_TäyttäjäUserId",
                table: "TehtavaVastaus");

            migrationBuilder.DropIndex(
                name: "IX_TehtavaVastaus_JatkajaUserId",
                table: "TehtavaVastaus");

            migrationBuilder.DropIndex(
                name: "IX_TehtavaVastaus_TarkistajaUserId",
                table: "TehtavaVastaus");

            migrationBuilder.DropIndex(
                name: "IX_TehtavaVastaus_TäyttäjäUserId",
                table: "TehtavaVastaus");

            migrationBuilder.AlterColumn<int>(
                name: "TäyttäjäUserId",
                table: "TehtavaVastaus",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TarkistajaUserId",
                table: "TehtavaVastaus",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "JatkajaUserId",
                table: "TehtavaVastaus",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
