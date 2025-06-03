using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faoem.FacilityStatus.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class _000000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "facility_status_facility_status_binding",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    binding_type = table.Column<int>(type: "INTEGER", nullable: false),
                    connector_instance = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    connection_name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    data_point = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    facility_id = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_facility_status_facility_status_binding", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "facility_status_facility_status_binding");
        }
    }
}
