using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faoem.Shift.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class _000000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "shift_shift",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false, comment: "班次 Id")
                        .Annotation("Sqlite:Autoincrement", true),
                    number = table.Column<byte>(type: "INTEGER", nullable: false, comment: "班次编号"),
                    is_enabled = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否启用"),
                    name = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false, comment: "班次名称"),
                    start_time = table.Column<long>(type: "INTEGER", nullable: false, comment: "开始时间，unix 时间戳"),
                    end_time = table.Column<long>(type: "INTEGER", nullable: false, comment: "结束时间，unix 时间戳"),
                    span_the_day = table.Column<bool>(type: "INTEGER", nullable: false, comment: "是否跨天")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shift_shift", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shift_shift");
        }
    }
}
