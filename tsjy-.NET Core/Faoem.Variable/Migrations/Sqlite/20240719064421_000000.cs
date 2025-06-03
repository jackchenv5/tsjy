using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faoem.Variable.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class _000000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "variable_archived_variable",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    connector_instance = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    connection_name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    data_point = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    definition_id = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    DataType = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    archive_type = table.Column<byte>(type: "INTEGER", nullable: false),
                    archive_mode = table.Column<byte>(type: "INTEGER", nullable: false),
                    archive_interval = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_variable_archived_variable", x => x.id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "variable_archived_variable_module_map");

            migrationBuilder.DropTable(
                name: "variable_archived_variable");
        }
    }
}
