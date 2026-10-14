using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrafikCezaYonetimApp.Migrations
{
    /// <inheritdoc />
    public partial class OdemeEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Penalties",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "Penalties",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Penalties");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "Penalties");
        }
    }
}
