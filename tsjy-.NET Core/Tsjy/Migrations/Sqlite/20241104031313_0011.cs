using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsjy.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class _0011 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "connection_name",
                table: "tsjy_craft_binding",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "connector_instance",
                table: "tsjy_craft_binding",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "data_point",
                table: "tsjy_craft_binding",
                type: "TEXT",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "tsjy_craft_binding",
                type: "TEXT",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "connection_name",
                table: "tsjy_craft_binding");

            migrationBuilder.DropColumn(
                name: "connector_instance",
                table: "tsjy_craft_binding");

            migrationBuilder.DropColumn(
                name: "data_point",
                table: "tsjy_craft_binding");

            migrationBuilder.DropColumn(
                name: "name",
                table: "tsjy_craft_binding");
        }
    }
}
