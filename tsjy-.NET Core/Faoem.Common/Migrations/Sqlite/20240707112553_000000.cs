using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Faoem.Common.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class _000000 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "common_captcha",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    lower_email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    code = table.Column<int>(type: "INTEGER", nullable: false),
                    expire_time = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_common_captcha", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "common_menu",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    label = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    lower_label = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    is_sub_menu = table.Column<bool>(type: "INTEGER", nullable: false),
                    order = table.Column<int>(type: "INTEGER", nullable: false),
                    parent_id = table.Column<long>(type: "INTEGER", nullable: true),
                    route = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    lower_route = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_common_menu", x => x.id);
                    table.ForeignKey(
                        name: "FK_common_menu_common_menu_parent_id",
                        column: x => x.parent_id,
                        principalTable: "common_menu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "common_permission",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    route = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    http_method = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    controller_name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    action_description = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_common_permission", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "common_role",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    role_name = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    lower_role_name = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_common_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "common_setting",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    key = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    value = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_common_setting", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "common_user",
                columns: table => new
                {
                    id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    username = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    lower_username = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    full_name = table.Column<string>(type: "TEXT", maxLength: 32, nullable: true),
                    created_at = table.Column<long>(type: "INTEGER", nullable: false),
                    last_login = table.Column<long>(type: "INTEGER", nullable: false),
                    password_hash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    salt = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false),
                    email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    lower_email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_common_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "common_role_menu",
                columns: table => new
                {
                    role_id = table.Column<long>(type: "INTEGER", nullable: false),
                    menu_id = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_common_role_menu", x => new { x.role_id, x.menu_id });
                    table.ForeignKey(
                        name: "FK_common_role_menu_common_menu_menu_id",
                        column: x => x.menu_id,
                        principalTable: "common_menu",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_common_role_menu_common_role_role_id",
                        column: x => x.role_id,
                        principalTable: "common_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "common_role_permission",
                columns: table => new
                {
                    role_id = table.Column<long>(type: "INTEGER", nullable: false),
                    permission_id = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_common_role_permission", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "FK_common_role_permission_common_permission_permission_id",
                        column: x => x.permission_id,
                        principalTable: "common_permission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_common_role_permission_common_role_role_id",
                        column: x => x.role_id,
                        principalTable: "common_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "common_user_role",
                columns: table => new
                {
                    user_id = table.Column<long>(type: "INTEGER", nullable: false),
                    role_id = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_common_user_role", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_common_user_role_common_role_role_id",
                        column: x => x.role_id,
                        principalTable: "common_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_common_user_role_common_user_user_id",
                        column: x => x.user_id,
                        principalTable: "common_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_common_menu_parent_id",
                table: "common_menu",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_common_role_menu_menu_id",
                table: "common_role_menu",
                column: "menu_id");

            migrationBuilder.CreateIndex(
                name: "IX_common_role_permission_permission_id",
                table: "common_role_permission",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_common_user_role_role_id",
                table: "common_user_role",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "common_captcha");

            migrationBuilder.DropTable(
                name: "common_role_menu");

            migrationBuilder.DropTable(
                name: "common_role_permission");

            migrationBuilder.DropTable(
                name: "common_setting");

            migrationBuilder.DropTable(
                name: "common_user_role");

            migrationBuilder.DropTable(
                name: "common_menu");

            migrationBuilder.DropTable(
                name: "common_permission");

            migrationBuilder.DropTable(
                name: "common_role");

            migrationBuilder.DropTable(
                name: "common_user");
        }
    }
}
