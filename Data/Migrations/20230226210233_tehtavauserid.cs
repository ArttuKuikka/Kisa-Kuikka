using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kipaplus.Data.Migrations
{
    /// <inheritdoc />
    public partial class tehtavauserid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Access",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<int>(
                name: "JatkajaUserId",
                table: "TehtavaVastaus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TarkistajaUserId",
                table: "TehtavaVastaus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TäyttäjäUserId",
                table: "TehtavaVastaus",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JatkajaUserId",
                table: "TehtavaVastaus");

            migrationBuilder.DropColumn(
                name: "TarkistajaUserId",
                table: "TehtavaVastaus");

            migrationBuilder.DropColumn(
                name: "TäyttäjäUserId",
                table: "TehtavaVastaus");

            migrationBuilder.AddColumn<string>(
                name: "Access",
                table: "AspNetRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
