using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kipaplus.Data.Migrations
{
    /// <inheritdoc />
    public partial class Keskeytys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Keskeytetty",
                table: "Vartio",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Keskeytetty",
                table: "Vartio");
        }
    }
}
