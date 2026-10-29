using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VoenkomatSpo.Infra.Migrations
{
    public partial class AddGeneratedDocuments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GeneratedDocument",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    number = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    form_number = table.Column<int>(type: "integer", nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    details = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    commissariat_name = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    creator_login = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    formed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_reconciled = table.Column<bool>(type: "boolean", nullable: false),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    pdf_data = table.Column<byte[]>(type: "bytea", nullable: false),
                    student_id = table.Column<int>(type: "integer", nullable: true),
                    commissariat_id = table.Column<int>(type: "integer", nullable: true),
                    report_id = table.Column<int>(type: "integer", nullable: true),
                    formed_by = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedDocument", x => x.id);
                    table.CheckConstraint("CK_GeneratedDocument_Form", "form_number IN (6, 10)");
                    table.ForeignKey(
                        name: "FK_GeneratedDocument_MilitaryCommissariat_commissariat_id",
                        column: x => x.commissariat_id,
                        principalTable: "MilitaryCommissariat",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeneratedDocument_Report_report_id",
                        column: x => x.report_id,
                        principalTable: "Report",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeneratedDocument_Student_student_id",
                        column: x => x.student_id,
                        principalTable: "Student",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeneratedDocument_User_formed_by",
                        column: x => x.formed_by,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedDocument_commissariat_id",
                table: "GeneratedDocument",
                column: "commissariat_id");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedDocument_formed_at",
                table: "GeneratedDocument",
                column: "formed_at");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedDocument_formed_by",
                table: "GeneratedDocument",
                column: "formed_by");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedDocument_number",
                table: "GeneratedDocument",
                column: "number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedDocument_report_id",
                table: "GeneratedDocument",
                column: "report_id");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedDocument_student_id",
                table: "GeneratedDocument",
                column: "student_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneratedDocument");
        }
    }
}
