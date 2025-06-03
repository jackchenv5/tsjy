using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faoem.Variable.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class _000001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "variable_archived_variable_module_map");

            migrationBuilder.RenameColumn(
                name: "DataType",
                table: "variable_archived_variable",
                newName: "data_type");

            migrationBuilder.AddColumn<long>(
                name: "created_at",
                table: "variable_archived_variable",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "variable_archived_variable",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "variable_archived_variable");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "variable_archived_variable");

            migrationBuilder.RenameColumn(
                name: "data_type",
                table: "variable_archived_variable",
                newName: "DataType");

            migrationBuilder.CreateTable(
                name: "variable_archived_variable_module_map",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    archived_variable_id = table.Column<long>(type: "INTEGER", nullable: false),
                    module_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_variable_archived_variable_module_map", x => x.id);
                    table.ForeignKey(
                        name: "FK_variable_archived_variable_module_map_variable_archived_variable_archived_variable_id",
                        column: x => x.archived_variable_id,
                        principalTable: "variable_archived_variable",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_variable_archived_variable_module_map_archived_variable_id",
                table: "variable_archived_variable_module_map",
                column: "archived_variable_id");
        }
    }
}
