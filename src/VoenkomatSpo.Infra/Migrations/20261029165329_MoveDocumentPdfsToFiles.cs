using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoenkomatSpo.Infra.Migrations;

public partial class MoveDocumentPdfsToFiles : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            CREATE TABLE IF NOT EXISTS "__DocumentFileExport" (id integer PRIMARY KEY, filename text NOT NULL);
            DO $$ BEGIN
                IF EXISTS (SELECT 1 FROM "GeneratedDocument" d WHERE NOT EXISTS (SELECT 1 FROM "__DocumentFileExport" f WHERE f.id=d.id)) THEN
                    RAISE EXCEPTION 'Export existing PDFs first: dotnet run --project tools/VoenkomatSpo.DbUpdate';
                END IF;
            END $$;
            """);
        migrationBuilder.AddColumn<string>(name: "pdf_path", table: "GeneratedDocument", type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.Sql("""
            UPDATE "GeneratedDocument" d SET pdf_path=f.filename FROM "__DocumentFileExport" f WHERE f.id=d.id;
            DROP TABLE "__DocumentFileExport";
            """);
        migrationBuilder.DropColumn(name: "pdf_data", table: "GeneratedDocument");
    }

    protected override void Down(MigrationBuilder migrationBuilder) =>
        throw new NotSupportedException("Restore a database backup to revert the PDF file migration.");
}
