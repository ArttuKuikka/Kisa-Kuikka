using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KisaKuikka.Data.Migrations
{
    /// <inheritdoc />
    public partial class _41220223 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "tilanne",
                table: "Vartio",
                newName: "Tilanne");

            migrationBuilder.RenameColumn(
                name: "vartionMinimikoko",
                table: "Sarja",
                newName: "VartionMinimikoko");

            migrationBuilder.RenameColumn(
                name: "vartionMaksimiko",
                table: "Sarja",
                newName: "VartionMaksimiko");

            migrationBuilder.RenameColumn(
                name: "kisaId",
                table: "Sarja",
                newName: "KisaId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Kisa",
                newName: "Nimi");

            migrationBuilder.CreateTable(
                name: "Rasti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SarjaId = table.Column<int>(type: "int", nullable: false),
                    KisaId = table.Column<int>(type: "int", nullable: false),
                    Nimi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OhjeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rasti", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rasti");

            migrationBuilder.RenameColumn(
                name: "Tilanne",
                table: "Vartio",
                newName: "tilanne");

            migrationBuilder.RenameColumn(
                name: "VartionMinimikoko",
                table: "Sarja",
                newName: "vartionMinimikoko");

            migrationBuilder.RenameColumn(
                name: "VartionMaksimiko",
                table: "Sarja",
                newName: "vartionMaksimiko");

            migrationBuilder.RenameColumn(
                name: "KisaId",
                table: "Sarja",
                newName: "kisaId");

            migrationBuilder.RenameColumn(
                name: "Nimi",
                table: "Kisa",
                newName: "Name");
        }
    }
}
