using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faoem.Facility.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class _000000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "facility_facility",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    is_enabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    serial_number = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    description = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facility_facility", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "facility_facility");
        }
    }
}
