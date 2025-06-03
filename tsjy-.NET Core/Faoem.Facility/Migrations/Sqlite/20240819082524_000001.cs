using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faoem.Facility.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class _000001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "lower_name",
                table: "facility_facility",
                type: "TEXT",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "lower_serial_number",
                table: "facility_facility",
                type: "TEXT",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lower_name",
                table: "facility_facility");

            migrationBuilder.DropColumn(
                name: "lower_serial_number",
                table: "facility_facility");
        }
    }
}
