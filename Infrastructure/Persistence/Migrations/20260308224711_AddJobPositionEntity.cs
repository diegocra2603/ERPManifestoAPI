using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddJobPositionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "job_position_id",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "job_positions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    hourly_cost = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    audit_field_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    audit_field_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    audit_field_is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_job_positions", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("41d21bf1-155c-43c3-8858-b5b4d047c6ed"),
                column: "job_position_id",
                value: null);

            migrationBuilder.CreateIndex(
                name: "ix_users_job_position_id",
                table: "users",
                column: "job_position_id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_job_positions_job_position_id",
                table: "users",
                column: "job_position_id",
                principalTable: "job_positions",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_job_positions_job_position_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "job_positions");

            migrationBuilder.DropIndex(
                name: "ix_users_job_position_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "job_position_id",
                table: "users");
        }
    }
}
