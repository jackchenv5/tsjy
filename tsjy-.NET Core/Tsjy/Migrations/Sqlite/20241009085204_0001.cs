using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsjy.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class _0001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tsjy_alarm_definition",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    connector_instance = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    connection_name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    data_point = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    facility_id = table.Column<long>(type: "INTEGER", nullable: false),
                    message = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false),
                    trigger_type = table.Column<int>(type: "INTEGER", nullable: false),
                    data_type = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    trigger_value = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tsjy_alarm_definition", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tsjy_alarm_definition");
        }
    }
}
