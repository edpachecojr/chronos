using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chronos.Agenda.Infrastructure.Data.EF.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDiscriminadoresParaOwnedEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "endereco_prestador__discriminator",
                table: "organizacoes",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "fuso_horario__discriminator",
                table: "organizacoes",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "local_endereco__discriminator",
                table: "agendamentos",
                type: "smallint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "endereco_prestador__discriminator",
                table: "organizacoes");

            migrationBuilder.DropColumn(
                name: "fuso_horario__discriminator",
                table: "organizacoes");

            migrationBuilder.DropColumn(
                name: "local_endereco__discriminator",
                table: "agendamentos");
        }
    }
}
