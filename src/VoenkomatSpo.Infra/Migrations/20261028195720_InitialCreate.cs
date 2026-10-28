using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VoenkomatSpo.Infra.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MilitaryCommissariat",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    address = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MilitaryCommissariat", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    access_rights = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "StudyGroup",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    department = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    graduation_year = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyGroup", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    registered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    role_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.id);
                    table.ForeignKey(
                        name: "FK_User_Role_role_id",
                        column: x => x.role_id,
                        principalTable: "Role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    personal_file_number = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    file_transferred_at = table.Column<DateOnly>(type: "date", nullable: true),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    patronymic = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    city_id = table.Column<int>(type: "integer", nullable: false),
                    street = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    house_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    post_index = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    passport_data = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    enrolled_at = table.Column<DateOnly>(type: "date", nullable: false),
                    group_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.id);
                    table.ForeignKey(
                        name: "FK_Student_City_city_id",
                        column: x => x.city_id,
                        principalTable: "City",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Student_StudyGroup_group_id",
                        column: x => x.group_id,
                        principalTable: "StudyGroup",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ActionLog",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    table_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    record_id = table.Column<int>(type: "integer", nullable: false),
                    recorded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionLog", x => x.id);
                    table.ForeignKey(
                        name: "FK_ActionLog_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    year = table.Column<int>(type: "integer", nullable: false),
                    formed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_reconciled = table.Column<bool>(type: "boolean", nullable: false),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    commissariat_id = table.Column<int>(type: "integer", nullable: false),
                    formed_by = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.id);
                    table.ForeignKey(
                        name: "FK_Report_MilitaryCommissariat_commissariat_id",
                        column: x => x.commissariat_id,
                        principalTable: "MilitaryCommissariat",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Report_User_formed_by",
                        column: x => x.formed_by,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChangeEvent",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    event_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    event_date = table.Column<DateOnly>(type: "date", nullable: false),
                    notified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deadline = table.Column<DateOnly>(type: "date", nullable: false),
                    sent_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    student_id = table.Column<int>(type: "integer", nullable: false),
                    notified_by = table.Column<int>(type: "integer", nullable: true),
                    commissariat_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeEvent", x => x.id);
                    table.ForeignKey(
                        name: "FK_ChangeEvent_MilitaryCommissariat_commissariat_id",
                        column: x => x.commissariat_id,
                        principalTable: "MilitaryCommissariat",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeEvent_Student_student_id",
                        column: x => x.student_id,
                        principalTable: "Student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeEvent_User_notified_by",
                        column: x => x.notified_by,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonalCard",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    record_category = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    fitness_category = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    military_specialty = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    document_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    document_number = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    document_issued_at = table.Column<DateOnly>(type: "date", nullable: true),
                    document_requested_at = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_checked = table.Column<bool>(type: "boolean", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_archived = table.Column<bool>(type: "boolean", nullable: false),
                    removed_at = table.Column<DateOnly>(type: "date", nullable: true),
                    student_id = table.Column<int>(type: "integer", nullable: false),
                    commissariat_id = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalCard", x => x.id);
                    table.ForeignKey(
                        name: "FK_PersonalCard_MilitaryCommissariat_commissariat_id",
                        column: x => x.commissariat_id,
                        principalTable: "MilitaryCommissariat",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonalCard_Student_student_id",
                        column: x => x.student_id,
                        principalTable: "Student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonalCard_User_created_by",
                        column: x => x.created_by,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionLog_recorded_at",
                table: "ActionLog",
                column: "recorded_at");

            migrationBuilder.CreateIndex(
                name: "IX_ActionLog_table_name_record_id",
                table: "ActionLog",
                columns: new[] { "table_name", "record_id" });

            migrationBuilder.CreateIndex(
                name: "IX_ActionLog_user_id",
                table: "ActionLog",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeEvent_commissariat_id",
                table: "ChangeEvent",
                column: "commissariat_id");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeEvent_deadline",
                table: "ChangeEvent",
                column: "deadline");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeEvent_notified_by",
                table: "ChangeEvent",
                column: "notified_by");

            migrationBuilder.CreateIndex(
                name: "IX_ChangeEvent_student_id",
                table: "ChangeEvent",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_City_name",
                table: "City",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonalCard_commissariat_id",
                table: "PersonalCard",
                column: "commissariat_id");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalCard_created_by",
                table: "PersonalCard",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalCard_student_id",
                table: "PersonalCard",
                column: "student_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Report_commissariat_id",
                table: "Report",
                column: "commissariat_id");

            migrationBuilder.CreateIndex(
                name: "IX_Report_formed_by",
                table: "Report",
                column: "formed_by");

            migrationBuilder.CreateIndex(
                name: "IX_Report_year_commissariat_id",
                table: "Report",
                columns: new[] { "year", "commissariat_id" });

            migrationBuilder.CreateIndex(
                name: "IX_Role_name",
                table: "Role",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Student_city_id",
                table: "Student",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_Student_group_id",
                table: "Student",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "IX_Student_personal_file_number",
                table: "Student",
                column: "personal_file_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudyGroup_name_graduation_year",
                table: "StudyGroup",
                columns: new[] { "name", "graduation_year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_login",
                table: "User",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_role_id",
                table: "User",
                column: "role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionLog");

            migrationBuilder.DropTable(
                name: "ChangeEvent");

            migrationBuilder.DropTable(
                name: "PersonalCard");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "MilitaryCommissariat");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "StudyGroup");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
