using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFiscalDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fiscal_documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    nit_receptor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nombre_receptor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    direccion_receptor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    tipo_venta = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false),
                    destino_venta = table.Column<int>(type: "integer", nullable: false),
                    fecha = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    moneda = table.Column<int>(type: "integer", nullable: false),
                    tasa = table.Column<decimal>(type: "numeric(10,6)", precision: 10, scale: 6, nullable: false),
                    referencia = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    numero_acceso = table.Column<long>(type: "bigint", nullable: true),
                    serie_admin = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    numero_admin = table.Column<long>(type: "bigint", nullable: true),
                    bruto = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    descuento = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    exento = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    otros = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    neto = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    isr = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    iva = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    total = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    serie = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    preimpreso = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    numero_autorizacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    doc_asociado_serie = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    doc_asociado_preimpreso = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    xml_enviado = table.Column<string>(type: "text", nullable: true),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fiscal_documents", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "fiscal_document_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    fiscal_document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    measure_unit = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(20,5)", precision: 20, scale: 5, nullable: false),
                    price = table.Column<decimal>(type: "numeric(20,7)", precision: 20, scale: 7, nullable: false),
                    discount_percentage = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    gross_amount = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    discount_amount = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    exempt_amount = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    other_taxes = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    net_amount = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    isr_amount = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    iva_amount = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    sale_type = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fiscal_document_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_fiscal_document_items_fiscal_documents_fiscal_document_id",
                        column: x => x.fiscal_document_id,
                        principalTable: "fiscal_documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_document_items_fiscal_document_id",
                table: "fiscal_document_items",
                column: "fiscal_document_id");

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_documents_numero_autorizacion",
                table: "fiscal_documents",
                column: "numero_autorizacion");

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_documents_referencia",
                table: "fiscal_documents",
                column: "referencia");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fiscal_document_items");

            migrationBuilder.DropTable(
                name: "fiscal_documents");
        }
    }
}
