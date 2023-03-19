using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kipaplus.Data.Migrations
{
    /// <inheritdoc />
    public partial class _19032023 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OhjeId",
                table: "Rasti",
                newName: "edellinenTilanneId");

            migrationBuilder.AddColumn<bool>(
                name: "Keskeytetty",
                table: "Vartio",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OdottaaTilanneHyvaksyntaa",
                table: "Rasti",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "nykyinenTilanneId",
                table: "Rasti",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tilanne",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KisaId = table.Column<int>(type: "int", nullable: false),
                    Nimi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TarvitseeHyvaksynnan = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tilanne", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tilanne");

            migrationBuilder.DropColumn(
                name: "Keskeytetty",
                table: "Vartio");

            migrationBuilder.DropColumn(
                name: "OdottaaTilanneHyvaksyntaa",
                table: "Rasti");

            migrationBuilder.DropColumn(
                name: "nykyinenTilanneId",
                table: "Rasti");

            migrationBuilder.RenameColumn(
                name: "edellinenTilanneId",
                table: "Rasti",
                newName: "OhjeId");
        }
    }
}
