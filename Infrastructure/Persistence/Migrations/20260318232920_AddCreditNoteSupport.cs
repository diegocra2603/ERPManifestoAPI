using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditNoteSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "original_invoice_id",
                table: "invoices",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_invoices_original_invoice_id",
                table: "invoices",
                column: "original_invoice_id");

            migrationBuilder.AddForeignKey(
                name: "fk_invoices_invoices_original_invoice_id",
                table: "invoices",
                column: "original_invoice_id",
                principalTable: "invoices",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_invoices_invoices_original_invoice_id",
                table: "invoices");

            migrationBuilder.DropIndex(
                name: "ix_invoices_original_invoice_id",
                table: "invoices");

            migrationBuilder.DropColumn(
                name: "original_invoice_id",
                table: "invoices");
        }
    }
}
