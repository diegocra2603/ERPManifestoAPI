using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBriefFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop Brief tables
            migrationBuilder.DropTable(
                name: "brief_items");

            migrationBuilder.DropTable(
                name: "briefs");

            migrationBuilder.DropTable(
                name: "brief_questions");

            // Remove Brief role_permissions (SuperAdmin)
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("55e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("66f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("77a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("88b8c9d0-e1f2-4a3b-4c5d-6e7f8a9b0c1d"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            // Remove Brief role_permissions (Admin)
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("55e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("66f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("77a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            // Remove Brief permissions
            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("55e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("66f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("77a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("88b8c9d0-e1f2-4a3b-4c5d-6e7f8a9b0c1d"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("11a1b2c3-d4e5-4f6a-7b8c-9d0e1f2a3b4c"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("22b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("33c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

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
                keyValues: new object[] { new Guid("11a1b2c3-d4e5-4f6a-7b8c-9d0e1f2a3b4c"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("22b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("33c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("44d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("e5b6c7d8-f9a0-4e1f-2b3c-4d5e6f7a8b9c"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("f6c7d8e9-a0b1-4f2a-3c4d-5e6f7a8b9c0d"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.CreateTable(
                name: "briefs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    brand_perception = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    branding_elements_to_preserve = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    budget = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    client_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    communication_problems = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    company_background = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    contact_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    delivery_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    duration_months = table.Column<int>(type: "integer", nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_briefs", x => x.id);
                    table.ForeignKey(
                        name: "fk_briefs_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "brief_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brief_id = table.Column<Guid>(type: "uuid", nullable: false),
                    comments = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    is_selected = table.Column<bool>(type: "boolean", nullable: false),
                    item_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    section_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_brief_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_brief_items_briefs_brief_id",
                        column: x => x.brief_id,
                        principalTable: "briefs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "code", "description", "name", "audit_field_created_at", "audit_field_deleted_at", "audit_field_is_active", "audit_field_updated_at" },
                values: new object[,]
                {
                    { new Guid("55e5f6a7-b8c9-4d0e-1f2a-3b4c5d6e7f8a"), "Brief.Create", "Permite crear nuevos briefs", "Crear Brief", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("66f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"), "Brief.Read", "Permite ver información de briefs", "Ver Brief", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("77a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"), "Brief.Update", "Permite actualizar briefs", "Actualizar Brief", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("88b8c9d0-e1f2-4a3b-4c5d-6e7f8a9b0c1d"), "Brief.Delete", "Permite eliminar briefs", "Eliminar Brief", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null }
                });

            migrationBuilder.CreateIndex(
                name: "ix_brief_items_brief_id",
                table: "brief_items",
                column: "brief_id");

            migrationBuilder.CreateIndex(
                name: "ix_briefs_product_id",
                table: "briefs",
                column: "product_id");
        }
    }
}
