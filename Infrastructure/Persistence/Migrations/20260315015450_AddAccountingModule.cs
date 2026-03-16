using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "account_catalogs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    account_type = table.Column<int>(type: "integer", nullable: false),
                    nature = table.Column<int>(type: "integer", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    level = table.Column<int>(type: "integer", nullable: false),
                    accepts_movements = table.Column<bool>(type: "boolean", nullable: false),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_account_catalogs", x => x.id);
                    table.ForeignKey(
                        name: "fk_account_catalogs_account_catalogs_parent_id",
                        column: x => x.parent_id,
                        principalTable: "account_catalogs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "accounting_periods",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounting_periods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "currencies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    symbol = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    is_functional = table.Column<bool>(type: "boolean", nullable: false),
                    decimal_places = table.Column<int>(type: "integer", nullable: false),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currencies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tax_configurations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    percentage = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: false),
                    tax_type = table.Column<int>(type: "integer", nullable: false),
                    debit_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    credit_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tax_configurations", x => x.id);
                    table.ForeignKey(
                        name: "fk_tax_configurations_account_catalogs_credit_account_id",
                        column: x => x.credit_account_id,
                        principalTable: "account_catalogs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tax_configurations_account_catalogs_debit_account_id",
                        column: x => x.debit_account_id,
                        principalTable: "account_catalogs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "exchange_rates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    buy_rate = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    sell_rate = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    source = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exchange_rates", x => x.id);
                    table.ForeignKey(
                        name: "fk_exchange_rates_currencies_currency_id",
                        column: x => x.currency_id,
                        principalTable: "currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "journal_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    entry_number = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    entry_type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    period_id = table.Column<Guid>(type: "uuid", nullable: false),
                    currency_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exchange_rate = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_journal_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_journal_entries_accounting_periods_period_id",
                        column: x => x.period_id,
                        principalTable: "accounting_periods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_journal_entries_currencies_currency_id",
                        column: x => x.currency_id,
                        principalTable: "currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "journal_entry_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    debit = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    credit = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    debit_functional = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    credit_functional = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    line_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_journal_entry_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_journal_entry_lines_account_catalogs_account_id",
                        column: x => x.account_id,
                        principalTable: "account_catalogs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_journal_entry_lines_journal_entries_journal_entry_id",
                        column: x => x.journal_entry_id,
                        principalTable: "journal_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tax_transactions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tax_configuration_id = table.Column<Guid>(type: "uuid", nullable: false),
                    journal_entry_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fiscal_document_id = table.Column<Guid>(type: "uuid", nullable: true),
                    currency_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exchange_rate = table.Column<decimal>(type: "numeric(20,6)", precision: 20, scale: 6, nullable: false),
                    taxable_base = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    taxable_base_functional = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    tax_amount_functional = table.Column<decimal>(type: "numeric(20,2)", precision: 20, scale: 2, nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    transaction_type = table.Column<int>(type: "integer", nullable: false),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tax_transactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_tax_transactions_currencies_currency_id",
                        column: x => x.currency_id,
                        principalTable: "currencies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tax_transactions_fiscal_documents_fiscal_document_id",
                        column: x => x.fiscal_document_id,
                        principalTable: "fiscal_documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tax_transactions_journal_entries_journal_entry_id",
                        column: x => x.journal_entry_id,
                        principalTable: "journal_entries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_tax_transactions_tax_configurations_tax_configuration_id",
                        column: x => x.tax_configuration_id,
                        principalTable: "tax_configurations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "code", "description", "name", "audit_field_created_at", "audit_field_deleted_at", "audit_field_is_active", "audit_field_updated_at" },
                values: new object[,]
                {
                    { new Guid("55a1b2c3-d4e5-4f6a-7b8c-9d0e1f2a3b01"), "Accounting.Create", "Permite crear registros contables", "Crear Contabilidad", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("55a1b2c3-d4e5-4f6a-7b8c-9d0e1f2a3b02"), "Accounting.Read", "Permite ver información contable", "Ver Contabilidad", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("55a1b2c3-d4e5-4f6a-7b8c-9d0e1f2a3b03"), "Accounting.Update", "Permite actualizar registros contables", "Actualizar Contabilidad", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("55a1b2c3-d4e5-4f6a-7b8c-9d0e1f2a3b04"), "Accounting.Delete", "Permite eliminar registros contables", "Eliminar Contabilidad", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("55a1b2c3-d4e5-4f6a-7b8c-9d0e1f2a3b05"), "Accounting.Close", "Permite cerrar períodos contables", "Cerrar Período", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null }
                });

            migrationBuilder.CreateIndex(
                name: "ix_account_catalogs_account_code",
                table: "account_catalogs",
                column: "account_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_account_catalogs_parent_id",
                table: "account_catalogs",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_currencies_code",
                table: "currencies",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_exchange_rates_currency_id_date",
                table: "exchange_rates",
                columns: new[] { "currency_id", "date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_currency_id",
                table: "journal_entries",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_entry_number",
                table: "journal_entries",
                column: "entry_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_journal_entries_period_id",
                table: "journal_entries",
                column: "period_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_entry_lines_account_id",
                table: "journal_entry_lines",
                column: "account_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_entry_lines_journal_entry_id",
                table: "journal_entry_lines",
                column: "journal_entry_id");

            migrationBuilder.CreateIndex(
                name: "ix_tax_configurations_credit_account_id",
                table: "tax_configurations",
                column: "credit_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_tax_configurations_debit_account_id",
                table: "tax_configurations",
                column: "debit_account_id");

            migrationBuilder.CreateIndex(
                name: "ix_tax_transactions_currency_id",
                table: "tax_transactions",
                column: "currency_id");

            migrationBuilder.CreateIndex(
                name: "ix_tax_transactions_fiscal_document_id",
                table: "tax_transactions",
                column: "fiscal_document_id");

            migrationBuilder.CreateIndex(
                name: "ix_tax_transactions_journal_entry_id",
                table: "tax_transactions",
                column: "journal_entry_id");

            migrationBuilder.CreateIndex(
                name: "ix_tax_transactions_tax_configuration_id",
                table: "tax_transactions",
                column: "tax_configuration_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exchange_rates");

            migrationBuilder.DropTable(
                name: "journal_entry_lines");

            migrationBuilder.DropTable(
                name: "tax_transactions");

            migrationBuilder.DropTable(
                name: "journal_entries");

            migrationBuilder.DropTable(
                name: "tax_configurations");

            migrationBuilder.DropTable(
                name: "accounting_periods");

            migrationBuilder.DropTable(
                name: "currencies");

            migrationBuilder.DropTable(
                name: "account_catalogs");

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("55a1b2c3-d4e5-4f6a-7b8c-9d0e1f2a3b01"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("55a1b2c3-d4e5-4f6a-7b8c-9d0e1f2a3b02"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("55a1b2c3-d4e5-4f6a-7b8c-9d0e1f2a3b03"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("55a1b2c3-d4e5-4f6a-7b8c-9d0e1f2a3b04"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("55a1b2c3-d4e5-4f6a-7b8c-9d0e1f2a3b05"));
        }
    }
}
