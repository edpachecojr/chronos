using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronos.Agenda.Infrastructure.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarPapelAoMembroOrganizacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "papel",
                table: "membros_organizacao",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Proprietario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "papel",
                table: "membros_organizacao");
        }
    }
}
