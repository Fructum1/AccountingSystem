using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountingSystem.Migrations
{
    public partial class Initital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "position",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_position", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "request_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_request_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "request_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    paid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_request_type", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "unit",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unit", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    surname = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    patronymic = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    position = table.Column<int>(type: "int", nullable: false),
                    phone_number = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    unit = table.Column<int>(type: "int", nullable: false),
                    unused_vacation_days = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_user_position",
                        column: x => x.position,
                        principalTable: "position",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_unit",
                        column: x => x.unit,
                        principalTable: "unit",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "request",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    request_type = table.Column<int>(type: "int", nullable: false),
                    sending_date = table.Column<DateTime>(type: "date", nullable: true),
                    creation_date = table.Column<DateTime>(type: "date", nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    sender = table.Column<int>(type: "int", nullable: false),
                    portability_date = table.Column<bool>(type: "bit", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    substitute_employee = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_request", x => x.id);
                    table.ForeignKey(
                        name: "FK_request_request_status",
                        column: x => x.status,
                        principalTable: "request_status",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_request_request_type",
                        column: x => x.request_type,
                        principalTable: "request_type",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_request_user",
                        column: x => x.sender,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_role", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_user_role_role",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_role_user",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "request_approver",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    request_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_request_approver", x => new { x.user_id, x.request_id });
                    table.ForeignKey(
                        name: "FK_request_approver_request",
                        column: x => x.request_id,
                        principalTable: "request",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_request_approver_user",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_request_request_type",
                table: "request",
                column: "request_type");

            migrationBuilder.CreateIndex(
                name: "IX_request_sender",
                table: "request",
                column: "sender");

            migrationBuilder.CreateIndex(
                name: "IX_request_status",
                table: "request",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_request_approver_request_id",
                table: "request_approver",
                column: "request_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_position",
                table: "user",
                column: "position");

            migrationBuilder.CreateIndex(
                name: "IX_user_unit",
                table: "user",
                column: "unit");

            migrationBuilder.CreateIndex(
                name: "IX_user_role_role_id",
                table: "user_role",
                column: "role_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "request_approver");

            migrationBuilder.DropTable(
                name: "user_role");

            migrationBuilder.DropTable(
                name: "request");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "request_status");

            migrationBuilder.DropTable(
                name: "request_type");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "position");

            migrationBuilder.DropTable(
                name: "unit");
        }
    }
}
