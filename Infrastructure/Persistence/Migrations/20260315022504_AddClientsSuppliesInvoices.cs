using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddClientsSuppliesInvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "suppliers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_suppliers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "invoices",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    invoice_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    due_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    client_id = table.Column<Guid>(type: "uuid", nullable: true),
                    supplier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    nit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    currency_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exchange_rate = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    total = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fiscal_serie = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    fiscal_numero = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    fiscal_autorizacion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoices", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoices_clients_client_id",
                        column: x => x.client_id,
                        principalTable: "clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_invoices_currencies_currency_id",
                        column: x => x.currency_id,
                        principalTable: "currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_invoices_journal_entries_journal_entry_id",
                        column: x => x.journal_entry_id,
                        principalTable: "journal_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_invoices_suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "suppliers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "invoice_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoice_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(20,5)", precision: 20, scale: 5, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(20,7)", precision: 20, scale: 7, nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    total = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    line_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoice_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoice_items_invoices_invoice_id",
                        column: x => x.invoice_id,
                        principalTable: "invoices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "account_catalogs",
                columns: new[] { "id", "accepts_movements", "account_code", "account_type", "level", "name", "nature", "parent_id", "audit_field_created_at", "audit_field_deleted_at", "audit_field_is_active", "audit_field_updated_at" },
                values: new object[,]
                {
                    { new Guid("a0a1b2c3-0001-4000-b000-000000000001"), false, "1", 1, 1, "Activos", 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0001-4000-b000-000000000002"), false, "2", 2, 1, "Pasivos", 2, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0001-4000-b000-000000000003"), false, "3", 3, 1, "Capital", 2, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0001-4000-b000-000000000004"), false, "4", 4, 1, "Ingresos", 2, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0001-4000-b000-000000000005"), false, "5", 5, 1, "Gastos", 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0001-4000-b000-000000000006"), false, "6", 6, 1, "Costos", 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null }
                });

            migrationBuilder.InsertData(
                table: "accounting_periods",
                columns: new[] { "id", "end_date", "name", "start_date", "status", "audit_field_created_at", "audit_field_deleted_at", "audit_field_is_active", "audit_field_updated_at" },
                values: new object[] { new Guid("c0a1b2c3-0004-4000-a000-000000000001"), new DateTime(2026, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), "Año 2026", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null });

            migrationBuilder.InsertData(
                table: "currencies",
                columns: new[] { "id", "code", "decimal_places", "is_functional", "name", "symbol", "audit_field_created_at", "audit_field_deleted_at", "audit_field_is_active", "audit_field_updated_at" },
                values: new object[,]
                {
                    { new Guid("c0a1b2c3-0001-4000-a000-000000000001"), "GTQ", 2, true, "Quetzal Guatemalteco", "Q", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("c0a1b2c3-0001-4000-a000-000000000002"), "USD", 2, false, "Dólar Estadounidense", "$", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null }
                });

            migrationBuilder.InsertData(
                table: "account_catalogs",
                columns: new[] { "id", "accepts_movements", "account_code", "account_type", "level", "name", "nature", "parent_id", "audit_field_created_at", "audit_field_deleted_at", "audit_field_is_active", "audit_field_updated_at" },
                values: new object[,]
                {
                    { new Guid("a0a1b2c3-0002-4000-b000-000000000001"), false, "1.1", 1, 2, "Activo Corriente", 1, new Guid("a0a1b2c3-0001-4000-b000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0002-4000-b000-000000000002"), false, "1.2", 1, 2, "Activo No Corriente", 1, new Guid("a0a1b2c3-0001-4000-b000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0002-4000-b000-000000000003"), false, "2.1", 2, 2, "Pasivo Corriente", 2, new Guid("a0a1b2c3-0001-4000-b000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0002-4000-b000-000000000004"), false, "2.2", 2, 2, "Pasivo No Corriente", 2, new Guid("a0a1b2c3-0001-4000-b000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0002-4000-b000-000000000005"), false, "3.1", 3, 2, "Capital Social", 2, new Guid("a0a1b2c3-0001-4000-b000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0002-4000-b000-000000000006"), false, "3.2", 3, 2, "Resultados Acumulados", 2, new Guid("a0a1b2c3-0001-4000-b000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0002-4000-b000-000000000007"), false, "4.1", 4, 2, "Ingresos Operativos", 2, new Guid("a0a1b2c3-0001-4000-b000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0002-4000-b000-000000000008"), false, "4.2", 4, 2, "Ingresos No Operativos", 2, new Guid("a0a1b2c3-0001-4000-b000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0002-4000-b000-000000000009"), false, "5.1", 5, 2, "Gastos de Operación", 1, new Guid("a0a1b2c3-0001-4000-b000-000000000005"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0002-4000-b000-000000000010"), false, "5.2", 5, 2, "Gastos de Administración", 1, new Guid("a0a1b2c3-0001-4000-b000-000000000005"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0002-4000-b000-000000000011"), false, "5.3", 5, 2, "Gastos Financieros", 1, new Guid("a0a1b2c3-0001-4000-b000-000000000005"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0002-4000-b000-000000000012"), false, "6.1", 6, 2, "Costo de Ventas", 1, new Guid("a0a1b2c3-0001-4000-b000-000000000006"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null }
                });

            migrationBuilder.InsertData(
                table: "exchange_rates",
                columns: new[] { "id", "buy_rate", "currency_id", "date", "sell_rate", "source", "audit_field_created_at", "audit_field_deleted_at", "audit_field_is_active", "audit_field_updated_at" },
                values: new object[] { new Guid("c0a1b2c3-0002-4000-a000-000000000001"), 7.66m, new Guid("c0a1b2c3-0001-4000-a000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 7.66m, "Manual", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null });

            migrationBuilder.InsertData(
                table: "account_catalogs",
                columns: new[] { "id", "accepts_movements", "account_code", "account_type", "level", "name", "nature", "parent_id", "audit_field_created_at", "audit_field_deleted_at", "audit_field_is_active", "audit_field_updated_at" },
                values: new object[,]
                {
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000001"), true, "1.1.01", 1, 3, "Caja General", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000002"), true, "1.1.02", 1, 3, "Bancos Nacional", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000003"), true, "1.1.03", 1, 3, "Cuentas por Cobrar", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000004"), true, "1.1.04", 1, 3, "IVA por Cobrar (Crédito Fiscal)", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000005"), true, "1.1.05", 1, 3, "Inventarios", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000006"), true, "1.1.06", 1, 3, "Anticipos a Proveedores", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000007"), true, "1.2.01", 1, 3, "Mobiliario y Equipo", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000008"), true, "1.2.02", 1, 3, "Equipo de Cómputo", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000009"), true, "1.2.03", 1, 3, "Vehículos", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000010"), true, "1.2.04", 1, 3, "Depreciación Acumulada", 2, new Guid("a0a1b2c3-0002-4000-b000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000011"), true, "2.1.01", 2, 3, "Cuentas por Pagar", 2, new Guid("a0a1b2c3-0002-4000-b000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000012"), true, "2.1.02", 2, 3, "IVA por Pagar (Débito Fiscal)", 2, new Guid("a0a1b2c3-0002-4000-b000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000013"), true, "2.1.03", 2, 3, "ISR por Pagar", 2, new Guid("a0a1b2c3-0002-4000-b000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000014"), true, "2.1.04", 2, 3, "Sueldos por Pagar", 2, new Guid("a0a1b2c3-0002-4000-b000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000015"), true, "2.1.05", 2, 3, "Retenciones de ISR", 2, new Guid("a0a1b2c3-0002-4000-b000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000016"), true, "2.2.01", 2, 3, "Préstamos Bancarios", 2, new Guid("a0a1b2c3-0002-4000-b000-000000000004"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000017"), true, "3.1.01", 3, 3, "Capital Autorizado", 2, new Guid("a0a1b2c3-0002-4000-b000-000000000005"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000018"), true, "3.1.02", 3, 3, "Reserva Legal", 2, new Guid("a0a1b2c3-0002-4000-b000-000000000005"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000019"), true, "3.2.01", 3, 3, "Utilidad del Ejercicio", 2, new Guid("a0a1b2c3-0002-4000-b000-000000000006"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000020"), true, "3.2.02", 3, 3, "Utilidades Retenidas", 2, new Guid("a0a1b2c3-0002-4000-b000-000000000006"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000021"), true, "4.1.01", 4, 3, "Ventas de Bienes", 2, new Guid("a0a1b2c3-0002-4000-b000-000000000007"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000022"), true, "4.1.02", 4, 3, "Ventas de Servicios", 2, new Guid("a0a1b2c3-0002-4000-b000-000000000007"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000023"), true, "4.2.01", 4, 3, "Otros Ingresos", 2, new Guid("a0a1b2c3-0002-4000-b000-000000000008"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000024"), true, "4.2.02", 4, 3, "Ingresos Financieros", 2, new Guid("a0a1b2c3-0002-4000-b000-000000000008"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000025"), true, "5.1.01", 5, 3, "Sueldos y Salarios", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000009"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000026"), true, "5.1.02", 5, 3, "Alquiler de Local", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000009"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000027"), true, "5.1.03", 5, 3, "Servicios Básicos", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000009"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000028"), true, "5.1.04", 5, 3, "Depreciaciones", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000009"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000029"), true, "5.2.01", 5, 3, "Sueldos Administrativos", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000010"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000030"), true, "5.2.02", 5, 3, "Alquiler Oficina", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000010"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000031"), true, "5.2.03", 5, 3, "Servicios Administrativos", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000010"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000032"), true, "5.3.01", 5, 3, "Intereses Bancarios", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000011"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000033"), true, "5.3.02", 5, 3, "Comisiones Bancarias", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000011"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000034"), true, "5.3.03", 5, 3, "Diferencial Cambiario", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000011"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000035"), true, "6.1.01", 6, 3, "Compras de Mercancías", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000012"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a0a1b2c3-0003-4000-b000-000000000036"), true, "6.1.02", 6, 3, "Costo de Materia Prima", 1, new Guid("a0a1b2c3-0002-4000-b000-000000000012"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null }
                });

            migrationBuilder.InsertData(
                table: "tax_configurations",
                columns: new[] { "id", "credit_account_id", "debit_account_id", "name", "percentage", "tax_type", "audit_field_created_at", "audit_field_deleted_at", "audit_field_is_active", "audit_field_updated_at" },
                values: new object[,]
                {
                    { new Guid("c0a1b2c3-0003-4000-c000-000000000001"), new Guid("a0a1b2c3-0003-4000-b000-000000000012"), new Guid("a0a1b2c3-0003-4000-b000-000000000004"), "IVA 12%", 12.00m, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("c0a1b2c3-0003-4000-c000-000000000002"), new Guid("a0a1b2c3-0003-4000-b000-000000000015"), new Guid("a0a1b2c3-0003-4000-b000-000000000013"), "ISR Retención 5%", 5.00m, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null }
                });

            migrationBuilder.CreateIndex(
                name: "ix_clients_nit",
                table: "clients",
                column: "nit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_invoice_items_invoice_id",
                table: "invoice_items",
                column: "invoice_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_client_id",
                table: "invoices",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_currency_id",
                table: "invoices",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_invoice_number",
                table: "invoices",
                column: "invoice_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_invoices_journal_entry_id",
                table: "invoices",
                column: "journal_entry_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_supplier_id",
                table: "invoices",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "ix_suppliers_nit",
                table: "suppliers",
                column: "nit",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "invoice_items");

            migrationBuilder.DropTable(
                name: "invoices");

            migrationBuilder.DropTable(
                name: "clients");

            migrationBuilder.DropTable(
                name: "suppliers");

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000001"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000002"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000003"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000005"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000006"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000007"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000008"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000009"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000010"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000011"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000014"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000016"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000017"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000018"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000019"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000020"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000021"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000022"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000023"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000024"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000025"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000026"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000027"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000028"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000029"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000030"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000031"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000032"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000033"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000034"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000035"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000036"));

            migrationBuilder.DeleteData(
                table: "accounting_periods",
                keyColumn: "id",
                keyValue: new Guid("c0a1b2c3-0004-4000-a000-000000000001"));

            migrationBuilder.DeleteData(
                table: "currencies",
                keyColumn: "id",
                keyValue: new Guid("c0a1b2c3-0001-4000-a000-000000000001"));

            migrationBuilder.DeleteData(
                table: "exchange_rates",
                keyColumn: "id",
                keyValue: new Guid("c0a1b2c3-0002-4000-a000-000000000001"));

            migrationBuilder.DeleteData(
                table: "tax_configurations",
                keyColumn: "id",
                keyValue: new Guid("c0a1b2c3-0003-4000-c000-000000000001"));

            migrationBuilder.DeleteData(
                table: "tax_configurations",
                keyColumn: "id",
                keyValue: new Guid("c0a1b2c3-0003-4000-c000-000000000002"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0002-4000-b000-000000000002"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0002-4000-b000-000000000004"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0002-4000-b000-000000000005"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0002-4000-b000-000000000006"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0002-4000-b000-000000000007"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0002-4000-b000-000000000008"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0002-4000-b000-000000000009"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0002-4000-b000-000000000010"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0002-4000-b000-000000000011"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0002-4000-b000-000000000012"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000004"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000012"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000013"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0003-4000-b000-000000000015"));

            migrationBuilder.DeleteData(
                table: "currencies",
                keyColumn: "id",
                keyValue: new Guid("c0a1b2c3-0001-4000-a000-000000000002"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0001-4000-b000-000000000003"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0001-4000-b000-000000000004"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0001-4000-b000-000000000005"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0001-4000-b000-000000000006"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0002-4000-b000-000000000001"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0002-4000-b000-000000000003"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0001-4000-b000-000000000001"));

            migrationBuilder.DeleteData(
                table: "account_catalogs",
                keyColumn: "id",
                keyValue: new Guid("a0a1b2c3-0001-4000-b000-000000000002"));
        }
    }
}
