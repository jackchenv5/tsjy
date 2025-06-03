using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsjy.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class _0016 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "process_method",
                table: "tsjy_part_maintain_history",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reason",
                table: "tsjy_part_maintain_history",
                type: "TEXT",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "process_method",
                table: "tsjy_part_maintain_history");

            migrationBuilder.DropColumn(
                name: "reason",
                table: "tsjy_part_maintain_history");
        }
    }
}
