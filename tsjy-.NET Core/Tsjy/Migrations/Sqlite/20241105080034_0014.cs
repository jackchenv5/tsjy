using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsjy.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class _0014 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tsjy_parts",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    part_name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    facility_id = table.Column<long>(type: "INTEGER", nullable: false),
                    remark = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    last_value = table.Column<double>(type: "REAL", nullable: true),
                    updated_at = table.Column<long>(type: "INTEGER", nullable: true),
                    connector_instance = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    connection_name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    data_point = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    variable_name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tsjy_parts", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tsjy_parts");
        }
    }
}
