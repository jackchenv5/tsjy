using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tsjy.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class _0015 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tsjy_part_maintain_history",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    part_name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    remark = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    facility_id = table.Column<long>(type: "INTEGER", nullable: false),
                    process_time = table.Column<long>(type: "INTEGER", nullable: false),
                    time = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tsjy_part_maintain_history", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tsjy_part_maintain_history");
        }
    }
}
