using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KisaKuikka.Data.Migrations
{
    /// <inheritdoc />
    public partial class DynamicAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SarjaId",
                table: "Rasti");

            migrationBuilder.AddColumn<string>(
                name: "TagSerial",
                table: "Vartio",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AddColumn<string>(
                name: "Access",
                table: "AspNetRoles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TagSkannaus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VartioId = table.Column<int>(type: "int", nullable: true),
                    RastiId = table.Column<int>(type: "int", nullable: false),
                    TagSerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isTulo = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagSkannaus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tehtava",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SarjaId = table.Column<int>(type: "int", nullable: false),
                    KisaId = table.Column<int>(type: "int", nullable: false),
                    RastiId = table.Column<int>(type: "int", nullable: false),
                    Nimi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TehtavaJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tehtava", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TehtavaVastaus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VartioId = table.Column<int>(type: "int", nullable: false),
                    SarjaId = table.Column<int>(type: "int", nullable: false),
                    KisaId = table.Column<int>(type: "int", nullable: false),
                    RastiId = table.Column<int>(type: "int", nullable: false),
                    TehtavaId = table.Column<int>(type: "int", nullable: false),
                    Kesken = table.Column<bool>(type: "bit", nullable: false),
                    TehtavaJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tarkistettu = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TehtavaVastaus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tiedosto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MimeType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiedosto", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagSkannaus");

            migrationBuilder.DropTable(
                name: "Tehtava");

            migrationBuilder.DropTable(
                name: "TehtavaVastaus");

            migrationBuilder.DropTable(
                name: "Tiedosto");

            migrationBuilder.DropColumn(
                name: "TagSerial",
                table: "Vartio");

            migrationBuilder.DropColumn(
                name: "Access",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<int>(
                name: "SarjaId",
                table: "Rasti",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
