using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JSDeposito.Api.Migrations
{
    /// <inheritdoc />
    public partial class tablePromoFrete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnderecoEntrega_Bairro",
                table: "Pedidos",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EnderecoEntrega_Cidade",
                table: "Pedidos",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "EnderecoEntrega_Latitude",
                table: "Pedidos",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "EnderecoEntrega_Longitude",
                table: "Pedidos",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnderecoEntrega_Numero",
                table: "Pedidos",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EnderecoEntrega_Rua",
                table: "Pedidos",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "FretePromocional",
                table: "Pedidos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Enderecos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PromocaoFretes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Inicio = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Fim = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Ativa = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromocaoFretes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromocaoFretes");

            migrationBuilder.DropColumn(
                name: "EnderecoEntrega_Bairro",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "EnderecoEntrega_Cidade",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "EnderecoEntrega_Latitude",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "EnderecoEntrega_Longitude",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "EnderecoEntrega_Numero",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "EnderecoEntrega_Rua",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "FretePromocional",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Enderecos");
        }
    }
}
