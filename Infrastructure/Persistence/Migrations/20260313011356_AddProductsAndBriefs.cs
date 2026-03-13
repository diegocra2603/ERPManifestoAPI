using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductsAndBriefs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "briefs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    contact_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    company_background = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    branding_elements_to_preserve = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    communication_problems = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    brand_perception = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    delivery_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    duration_months = table.Column<int>(type: "integer", nullable: true),
                    budget = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
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
                name: "product_job_positions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_position_id = table.Column<Guid>(type: "uuid", nullable: false),
                    hours = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_job_positions", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_job_positions_job_positions_job_position_id",
                        column: x => x.job_position_id,
                        principalTable: "job_positions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_product_job_positions_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "brief_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    brief_id = table.Column<Guid>(type: "uuid", nullable: false),
                    section_type = table.Column<int>(type: "integer", nullable: false),
                    item_name = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    is_selected = table.Column<bool>(type: "boolean", nullable: false),
                    comments = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
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
                    { new Guid("11a1b2c3-d4e5-4f6a-7b8c-9d0e1f2a3b4c"), "Product.Create", "Permite crear nuevos productos", "Crear Producto", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("22b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"), "Product.Read", "Permite ver información de productos", "Ver Producto", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("33c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"), "Product.Update", "Permite actualizar productos", "Actualizar Producto", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("44d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f"), "Product.Delete", "Permite eliminar productos", "Eliminar Producto", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
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

            migrationBuilder.CreateIndex(
                name: "ix_product_job_positions_job_position_id",
                table: "product_job_positions",
                column: "job_position_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_job_positions_product_id",
                table: "product_job_positions",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "brief_items");

            migrationBuilder.DropTable(
                name: "product_job_positions");

            migrationBuilder.DropTable(
                name: "briefs");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("11a1b2c3-d4e5-4f6a-7b8c-9d0e1f2a3b4c"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("22b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("33c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("44d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f"));

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
    }
}
