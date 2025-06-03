using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faoem.Variable.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class _000002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "definition_id",
                table: "variable_archived_variable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "definition_id",
                table: "variable_archived_variable",
                type: "TEXT",
                maxLength: 8,
                nullable: false,
                defaultValue: "");
        }
    }
}
