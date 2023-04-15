using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KisaKuikka.Data.Migrations
{
    /// <inheritdoc />
    public partial class vartiokisaid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KisaId",
                table: "Vartio",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KisaId",
                table: "Vartio");
        }
    }
}
