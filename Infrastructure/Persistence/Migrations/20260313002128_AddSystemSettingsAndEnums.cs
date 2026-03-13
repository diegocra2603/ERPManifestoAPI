using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSystemSettingsAndEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER TABLE fiscal_documents ALTER COLUMN tipo_venta TYPE integer USING tipo_venta::integer;");

            migrationBuilder.Sql(
                "ALTER TABLE fiscal_document_items ALTER COLUMN sale_type TYPE integer USING sale_type::integer;");

            migrationBuilder.CreateTable(
                name: "system_settings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_system_settings", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "code", "description", "name", "audit_field_created_at", "audit_field_deleted_at", "audit_field_is_active", "audit_field_updated_at" },
                values: new object[,]
                {
                    { new Guid("e5b6c7d8-f9a0-4e1f-2b3c-4d5e6f7a8b9c"), "Setting.Read", "Permite ver configuraciones del sistema", "Ver Configuración", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("f6c7d8e9-a0b1-4f2a-3c4d-5e6f7a8b9c0d"), "Setting.Update", "Permite actualizar configuraciones del sistema", "Actualizar Configuración", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission_id", "role_id" },
                values: new object[,]
                {
                    { new Guid("e5b6c7d8-f9a0-4e1f-2b3c-4d5e6f7a8b9c"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") },
                    { new Guid("f6c7d8e9-a0b1-4f2a-3c4d-5e6f7a8b9c0d"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") },
                    { new Guid("e5b6c7d8-f9a0-4e1f-2b3c-4d5e6f7a8b9c"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") },
                    { new Guid("f6c7d8e9-a0b1-4f2a-3c4d-5e6f7a8b9c0d"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") }
                });

            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                table: "system_settings",
                columns: new[] { "id", "key", "value", "description", "audit_field_created_at", "audit_field_deleted_at", "audit_field_is_active", "audit_field_updated_at" },
                values: new object[,]
                {
                    { new Guid("a1000001-0000-0000-0000-000000000001"), "Country", "Guatemala", "País del sistema", seedDate, null, true, null },
                    { new Guid("a1000001-0000-0000-0000-000000000002"), "Currency", "Quetzal", "Moneda principal del sistema", seedDate, null, true, null },
                    { new Guid("a1000001-0000-0000-0000-000000000003"), "CurrencySymbol", "Q", "Símbolo de la moneda principal", seedDate, null, true, null },
                    { new Guid("a1000001-0000-0000-0000-000000000004"), "ExchangeRateUSD", "7.66", "Tipo de cambio del dólar estadounidense", seedDate, null, true, null },
                    { new Guid("a1000001-0000-0000-0000-000000000005"), "TaxPercentage", "12", "Porcentaje de impuesto (IVA)", seedDate, null, true, null },
                    { new Guid("a1000001-0000-0000-0000-000000000006"), "IsISRWithholder", "false", "Indica si la empresa es retenedora de ISR", seedDate, null, true, null },
                    { new Guid("a1000001-0000-0000-0000-000000000007"), "IsQuarterlyISRAgent", "true", "Indica si la empresa es agente trimestral de ISR", seedDate, null, true, null },
                    { new Guid("a1000001-0000-0000-0000-000000000008"), "Timezone", "America/Guatemala", "Zona horaria del sistema", seedDate, null, true, null }
                });

            migrationBuilder.CreateIndex(
                name: "ix_system_settings_key",
                table: "system_settings",
                column: "key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "system_settings");

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("e5b6c7d8-f9a0-4e1f-2b3c-4d5e6f7a8b9c"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("f6c7d8e9-a0b1-4f2a-3c4d-5e6f7a8b9c0d"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("e5b6c7d8-f9a0-4e1f-2b3c-4d5e6f7a8b9c"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("f6c7d8e9-a0b1-4f2a-3c4d-5e6f7a8b9c0d"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("e5b6c7d8-f9a0-4e1f-2b3c-4d5e6f7a8b9c"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("f6c7d8e9-a0b1-4f2a-3c4d-5e6f7a8b9c0d"));

            migrationBuilder.AlterColumn<string>(
                name: "tipo_venta",
                table: "fiscal_documents",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "sale_type",
                table: "fiscal_document_items",
                type: "character varying(1)",
                maxLength: 1,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
