using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFiscalDataEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a1c4d7e8-b5f2-4a6b-c9d0-4e3f8a1b6c2d"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a3c6f2d9-b0e7-4f8a-c1b4-6d9e2f0a3b7c"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a5c8f4d1-b2e9-4f0a-c3b6-8d1e4f2a5b9c"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a7b8c9d0-e1f2-4a3b-4c5d-6e7f8a9b0c1d"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("b0d3a9e6-c7f4-4a5b-d8c1-3e6f9a7b0c4d"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("b4d7a3e0-c1f8-4a9b-d2c5-7e0f3a1b4c8d"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("b6d9a5e2-c3f0-4a1b-d4c7-9e2f5a3b6c0d"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("b8c9d0e1-f2a3-4b4c-5d6e-7f8a9b0c1d2e"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("b8f1a9d4-e5c2-4a7b-9f3e-1d6c8a4b2e5f"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("c1e4b0f7-d8a5-4b6c-e9d2-4f7a0b8c1d5e"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("c3d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("c5e8b4f1-d2a9-4b0c-e3d6-8f1a4b2c5d9e"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("c7e0b6f3-d4a1-4b2c-e5d8-0f3a6b4c7d1e"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("c9d0e1f2-a3b4-4c5d-6e7f-8a9b0c1d2e3f"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("c9e2b8f5-d6a3-4b8c-a0f4-2e7d9b5c3f6a"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("d0e1f2a3-b4c5-4d6e-7f8a-9b0c1d2e3f4a"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("d0f3c9a6-e7b4-4c9d-b1e5-3f8e0c6d4a7b"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("d2f5c1a8-e9b6-4c7d-f0e3-5a8b1c9d2e6f"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("d4e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("d8f1a4b5-e2c9-4d3e-f6a7-1b0c5d8e3f9a"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("e1a4d0b7-f8c5-4d0e-c2f6-4a9f1d7e5b8c"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("e1f2a3b4-c5d6-4e7f-8a9b-0c1d2e3f4a5b"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("e3a6d2b9-f0c7-4d8e-a1f4-6b9c2d0e3f7a"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("e9a2b5c6-f3d0-4e4f-a7b8-2c1d6e9f4a0b"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("f0b3c6d7-a4e1-4f5a-b8c9-3d2e7f0a5b1c"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("f2a3b4c5-d6e7-4f8a-9b0c-1d2e3f4a5b6c"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("f2b5e1c8-a9d6-4e3f-b7a0-5c8d1e9f2a6b"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("f4b7e3c0-a1d8-4e9f-b2a5-7c0d3e1f4a8b"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("f6a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"));

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a4d5e6f7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("b5e6f7a8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("c6f7a8b9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("d1a2b3c4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("d7a8b9c0-e1f2-4a3b-4c5d-6e7f8a9b0c1d"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("e2b3c4d5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("e8b9c0d1-f2a3-4b4c-5d6e-7f8a9b0c1d2e"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("f3c4d5e6-a7b8-4c9d-0e1f-2a3b4c5d6e7f"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("f9c0d1e2-a3b4-4c5d-6e7f-8a9b0c1d2e3f"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a4d5e6f7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("b5e6f7a8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("c6f7a8b9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("d1a2b3c4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("d7a8b9c0-e1f2-4a3b-4c5d-6e7f8a9b0c1d"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("e2b3c4d5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("e8b9c0d1-f2a3-4b4c-5d6e-7f8a9b0c1d2e"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("f3c4d5e6-a7b8-4c9d-0e1f-2a3b4c5d6e7f"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("f9c0d1e2-a3b4-4c5d-6e7f-8a9b0c1d2e3f"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a4d5e6f7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("b5e6f7a8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("c6f7a8b9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("d1a2b3c4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("d7a8b9c0-e1f2-4a3b-4c5d-6e7f8a9b0c1d"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("e2b3c4d5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("e8b9c0d1-f2a3-4b4c-5d6e-7f8a9b0c1d2e"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("f3c4d5e6-a7b8-4c9d-0e1f-2a3b4c5d6e7f"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("f9c0d1e2-a3b4-4c5d-6e7f-8a9b0c1d2e3f"));

            migrationBuilder.CreateTable(
                name: "fiscal_data_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    fiscal_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    fiscal_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fiscal_data_entries", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_fiscal_data_entries_fiscal_code",
                table: "fiscal_data_entries",
                column: "fiscal_code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "fiscal_data_entries");

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "code", "description", "name", "audit_field_created_at", "audit_field_deleted_at", "audit_field_is_active", "audit_field_updated_at" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"), "Store.Create", "Permite crear nuevas tiendas en el sistema", "Crear Tienda", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a1c4d7e8-b5f2-4a6b-c9d0-4e3f8a1b6c2d"), "Option.Delete", "Permite eliminar opciones del sistema", "Eliminar Opción", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a3c6f2d9-b0e7-4f8a-c1b4-6d9e2f0a3b7c"), "Category.Read", "Permite ver información de categorías", "Ver Categoría", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a4d5e6f7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"), "Check.Delete", "Permite cancelar cuentas del sistema", "Eliminar Cuenta", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a5c8f4d1-b2e9-4f0a-c3b6-8d1e4f2a5b9c"), "Ingredient.Read", "Permite ver información de ingredientes", "Ver Ingrediente", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("a7b8c9d0-e1f2-4a3b-4c5d-6e7f8a9b0c1d"), "StoreArea.Update", "Permite actualizar áreas de tienda", "Actualizar Área de Tienda", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("b0d3a9e6-c7f4-4a5b-d8c1-3e6f9a7b0c4d"), "ProductOption.Create", "Permite crear nuevas opciones de producto", "Crear Opción de Producto", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"), "Store.Read", "Permite ver información de tiendas", "Ver Tienda", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("b4d7a3e0-c1f8-4a9b-d2c5-7e0f3a1b4c8d"), "Category.Update", "Permite actualizar información de categorías", "Actualizar Categoría", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("b5e6f7a8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"), "CheckPayment.Create", "Permite registrar pagos en cuentas", "Crear Pago", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("b6d9a5e2-c3f0-4a1b-d4c7-9e2f5a3b6c0d"), "Ingredient.Update", "Permite actualizar información de ingredientes", "Actualizar Ingrediente", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("b8c9d0e1-f2a3-4b4c-5d6e-7f8a9b0c1d2e"), "StoreArea.Delete", "Permite eliminar áreas de tienda", "Eliminar Área de Tienda", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("b8f1a9d4-e5c2-4a7b-9f3e-1d6c8a4b2e5f"), "Product.Create", "Permite crear nuevos productos en el sistema", "Crear Producto", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("c1e4b0f7-d8a5-4b6c-e9d2-4f7a0b8c1d5e"), "ProductOption.Read", "Permite ver información de opciones de producto", "Ver Opción de Producto", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("c3d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f"), "Store.Update", "Permite actualizar información de tiendas", "Actualizar Tienda", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("c5e8b4f1-d2a9-4b0c-e3d6-8f1a4b2c5d9e"), "Category.Delete", "Permite eliminar categorías del sistema", "Eliminar Categoría", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("c6f7a8b9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"), "DayOpening.Create", "Permite registrar aperturas de día", "Apertura de Día", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("c7e0b6f3-d4a1-4b2c-e5d8-0f3a6b4c7d1e"), "Ingredient.Delete", "Permite eliminar ingredientes del sistema", "Eliminar Ingrediente", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("c9d0e1f2-a3b4-4c5d-6e7f-8a9b0c1d2e3f"), "Table.Create", "Permite crear nuevas mesas en el sistema", "Crear Mesa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("c9e2b8f5-d6a3-4b8c-a0f4-2e7d9b5c3f6a"), "Product.Read", "Permite ver información de productos", "Ver Producto", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("d0e1f2a3-b4c5-4d6e-7f8a-9b0c1d2e3f4a"), "Table.Read", "Permite ver información de mesas", "Ver Mesa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("d0f3c9a6-e7b4-4c9d-b1e5-3f8e0c6d4a7b"), "Product.Update", "Permite actualizar información de productos", "Actualizar Producto", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("d1a2b3c4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"), "Check.Create", "Permite crear nuevas cuentas en el sistema", "Crear Cuenta", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("d2f5c1a8-e9b6-4c7d-f0e3-5a8b1c9d2e6f"), "ProductOption.Update", "Permite actualizar opciones de producto", "Actualizar Opción de Producto", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("d4e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"), "Store.Delete", "Permite eliminar tiendas del sistema", "Eliminar Tienda", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("d7a8b9c0-e1f2-4a3b-4c5d-6e7f8a9b0c1d"), "DayOpening.Read", "Permite ver aperturas de día", "Ver Apertura de Día", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("d8f1a4b5-e2c9-4d3e-f6a7-1b0c5d8e3f9a"), "Option.Create", "Permite crear nuevas opciones en el sistema", "Crear Opción", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("e1a4d0b7-f8c5-4d0e-c2f6-4a9f1d7e5b8c"), "Product.Delete", "Permite eliminar productos del sistema", "Eliminar Producto", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("e1f2a3b4-c5d6-4e7f-8a9b-0c1d2e3f4a5b"), "Table.Update", "Permite actualizar información de mesas", "Actualizar Mesa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("e2b3c4d5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"), "Check.Read", "Permite ver información de cuentas", "Ver Cuenta", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("e3a6d2b9-f0c7-4d8e-a1f4-6b9c2d0e3f7a"), "ProductOption.Delete", "Permite eliminar opciones de producto", "Eliminar Opción de Producto", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"), "StoreArea.Create", "Permite crear nuevas áreas de tienda", "Crear Área de Tienda", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("e8b9c0d1-f2a3-4b4c-5d6e-7f8a9b0c1d2e"), "DayClosing.Create", "Permite registrar cierres de día", "Cierre de Día", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("e9a2b5c6-f3d0-4e4f-a7b8-2c1d6e9f4a0b"), "Option.Read", "Permite ver información de opciones", "Ver Opción", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("f0b3c6d7-a4e1-4f5a-b8c9-3d2e7f0a5b1c"), "Option.Update", "Permite actualizar opciones del sistema", "Actualizar Opción", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("f2a3b4c5-d6e7-4f8a-9b0c-1d2e3f4a5b6c"), "Table.Delete", "Permite eliminar mesas del sistema", "Eliminar Mesa", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("f2b5e1c8-a9d6-4e3f-b7a0-5c8d1e9f2a6b"), "Category.Create", "Permite crear nuevas categorías en el sistema", "Crear Categoría", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("f3c4d5e6-a7b8-4c9d-0e1f-2a3b4c5d6e7f"), "Check.Update", "Permite actualizar información de cuentas", "Actualizar Cuenta", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("f4b7e3c0-a1d8-4e9f-b2a5-7c0d3e1f4a8b"), "Ingredient.Create", "Permite crear nuevos ingredientes en el sistema", "Crear Ingrediente", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("f6a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"), "StoreArea.Read", "Permite ver información de áreas de tienda", "Ver Área de Tienda", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("f9c0d1e2-a3b4-4c5d-6e7f-8a9b0c1d2e3f"), "DayClosing.Read", "Permite ver cierres de día", "Ver Cierre de Día", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission_id", "role_id" },
                values: new object[,]
                {
                    { new Guid("a4d5e6f7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") },
                    { new Guid("b5e6f7a8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") },
                    { new Guid("c6f7a8b9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") },
                    { new Guid("d1a2b3c4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") },
                    { new Guid("d7a8b9c0-e1f2-4a3b-4c5d-6e7f8a9b0c1d"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") },
                    { new Guid("e2b3c4d5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") },
                    { new Guid("e8b9c0d1-f2a3-4b4c-5d6e-7f8a9b0c1d2e"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") },
                    { new Guid("f3c4d5e6-a7b8-4c9d-0e1f-2a3b4c5d6e7f"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") },
                    { new Guid("f9c0d1e2-a3b4-4c5d-6e7f-8a9b0c1d2e3f"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") },
                    { new Guid("a4d5e6f7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") },
                    { new Guid("b5e6f7a8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") },
                    { new Guid("c6f7a8b9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") },
                    { new Guid("d1a2b3c4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") },
                    { new Guid("d7a8b9c0-e1f2-4a3b-4c5d-6e7f8a9b0c1d"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") },
                    { new Guid("e2b3c4d5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") },
                    { new Guid("e8b9c0d1-f2a3-4b4c-5d6e-7f8a9b0c1d2e"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") },
                    { new Guid("f3c4d5e6-a7b8-4c9d-0e1f-2a3b4c5d6e7f"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") },
                    { new Guid("f9c0d1e2-a3b4-4c5d-6e7f-8a9b0c1d2e3f"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") }
                });
        }
    }
}
