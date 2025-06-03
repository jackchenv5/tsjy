using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsjy.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class _0009 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "facility_id",
                table: "tsjy_production_data",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "facility_id",
                table: "tsjy_production_data");
        }
    }
}
