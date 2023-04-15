using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KisaKuikka.Data.Migrations
{
    /// <inheritdoc />
    public partial class LiittymisId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LiittymisId",
                table: "Kisa",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LiittymisId",
                table: "Kisa");
        }
    }
}
