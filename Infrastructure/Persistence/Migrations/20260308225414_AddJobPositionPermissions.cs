using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddJobPositionPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "id", "code", "description", "name", "audit_field_created_at", "audit_field_deleted_at", "audit_field_is_active", "audit_field_updated_at" },
                values: new object[,]
                {
                    { new Guid("a1d2e3f4-b5c6-4a7b-8d9e-0f1a2b3c4d5e"), "JobPosition.Create", "Permite crear nuevos puestos de trabajo", "Crear Puesto de Trabajo", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("b2e3f4a5-c6d7-4b8c-9e0f-1a2b3c4d5e6f"), "JobPosition.Read", "Permite ver información de puestos de trabajo", "Ver Puesto de Trabajo", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("c3f4a5b6-d7e8-4c9d-0f1a-2b3c4d5e6f7a"), "JobPosition.Update", "Permite actualizar puestos de trabajo", "Actualizar Puesto de Trabajo", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null },
                    { new Guid("d4a5b6c7-e8f9-4d0e-1a2b-3c4d5e6f7a8b"), "JobPosition.Delete", "Permite eliminar puestos de trabajo", "Eliminar Puesto de Trabajo", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, true, null }
                });

            migrationBuilder.InsertData(
                table: "role_permissions",
                columns: new[] { "permission_id", "role_id" },
                values: new object[,]
                {
                    { new Guid("a1d2e3f4-b5c6-4a7b-8d9e-0f1a2b3c4d5e"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") },
                    { new Guid("b2e3f4a5-c6d7-4b8c-9e0f-1a2b3c4d5e6f"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") },
                    { new Guid("c3f4a5b6-d7e8-4c9d-0f1a-2b3c4d5e6f7a"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") },
                    { new Guid("a1d2e3f4-b5c6-4a7b-8d9e-0f1a2b3c4d5e"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") },
                    { new Guid("b2e3f4a5-c6d7-4b8c-9e0f-1a2b3c4d5e6f"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") },
                    { new Guid("c3f4a5b6-d7e8-4c9d-0f1a-2b3c4d5e6f7a"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") },
                    { new Guid("d4a5b6c7-e8f9-4d0e-1a2b-3c4d5e6f7a8b"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1d2e3f4-b5c6-4a7b-8d9e-0f1a2b3c4d5e"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("b2e3f4a5-c6d7-4b8c-9e0f-1a2b3c4d5e6f"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("c3f4a5b6-d7e8-4c9d-0f1a-2b3c4d5e6f7a"), new Guid("1159c308-279e-4bba-bae2-e8063c6d908e") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("a1d2e3f4-b5c6-4a7b-8d9e-0f1a2b3c4d5e"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("b2e3f4a5-c6d7-4b8c-9e0f-1a2b3c4d5e6f"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("c3f4a5b6-d7e8-4c9d-0f1a-2b3c4d5e6f7a"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "role_permissions",
                keyColumns: new[] { "permission_id", "role_id" },
                keyValues: new object[] { new Guid("d4a5b6c7-e8f9-4d0e-1a2b-3c4d5e6f7a8b"), new Guid("3f039871-c926-4313-b79c-8f17e622ec59") });

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("a1d2e3f4-b5c6-4a7b-8d9e-0f1a2b3c4d5e"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("b2e3f4a5-c6d7-4b8c-9e0f-1a2b3c4d5e6f"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("c3f4a5b6-d7e8-4c9d-0f1a-2b3c4d5e6f7a"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "id",
                keyValue: new Guid("d4a5b6c7-e8f9-4d0e-1a2b-3c4d5e6f7a8b"));
        }
    }
}
