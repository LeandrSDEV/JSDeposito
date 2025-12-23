using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JSDeposito.Api.Migrations
{
    /// <inheritdoc />
    public partial class TokenAnonimo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TokenAnonimo",
                table: "Pedidos",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenAnonimo",
                table: "Pedidos");
        }
    }
}
