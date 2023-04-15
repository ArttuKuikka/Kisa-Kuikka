using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KisaKuikka.Data.Migrations
{
    /// <inheritdoc />
    public partial class _4122022 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Kisa",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Sarja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nimi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    kisaId = table.Column<int>(type: "int", nullable: true),
                    vartionMaksimiko = table.Column<int>(type: "int", nullable: false),
                    vartionMinimikoko = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sarja", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vartio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nimi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: true),
                    SarjaId = table.Column<int>(type: "int", nullable: true),
                    Lippukunta = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tilanne = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vartio", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sarja");

            migrationBuilder.DropTable(
                name: "Vartio");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Kisa",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
