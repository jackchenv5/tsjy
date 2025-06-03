using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsjy.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class _0002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tsjy_alarm_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    alarm_definition_id = table.Column<long>(type: "INTEGER", nullable: false),
                    facility_id = table.Column<long>(type: "INTEGER", nullable: false),
                    start_time = table.Column<long>(type: "INTEGER", nullable: false),
                    end_time = table.Column<long>(type: "INTEGER", nullable: true),
                    duration = table.Column<long>(type: "INTEGER", nullable: true),
                    message = table.Column<string>(type: "TEXT", maxLength: 1024, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tsjy_alarm_history", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tsjy_alarm_history");
        }
    }
}
