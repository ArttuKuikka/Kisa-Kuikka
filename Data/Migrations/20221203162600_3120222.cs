using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kipaplus.Data.Migrations
{
    /// <inheritdoc />
    public partial class _3120222 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rasti");

            migrationBuilder.DropTable(
                name: "Vartio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rasti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KisaId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rasti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rasti_Kisa_KisaId",
                        column: x => x.KisaId,
                        principalTable: "Kisa",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Vartio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KisaId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vartio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vartio_Kisa_KisaId",
                        column: x => x.KisaId,
                        principalTable: "Kisa",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rasti_KisaId",
                table: "Rasti",
                column: "KisaId");

            migrationBuilder.CreateIndex(
                name: "IX_Vartio_KisaId",
                table: "Vartio",
                column: "KisaId");
        }
    }
}
