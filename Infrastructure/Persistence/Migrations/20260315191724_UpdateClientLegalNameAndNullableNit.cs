using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClientLegalNameAndNullableNit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_clients_nit",
                table: "clients");

            migrationBuilder.AlterColumn<string>(
                name: "nit",
                table: "clients",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "legal_name",
                table: "clients",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_clients_nit",
                table: "clients",
                column: "nit",
                unique: true,
                filter: "nit IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_clients_nit",
                table: "clients");

            migrationBuilder.DropColumn(
                name: "legal_name",
                table: "clients");

            migrationBuilder.AlterColumn<string>(
                name: "nit",
                table: "clients",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_clients_nit",
                table: "clients",
                column: "nit",
                unique: true);
        }
    }
}
