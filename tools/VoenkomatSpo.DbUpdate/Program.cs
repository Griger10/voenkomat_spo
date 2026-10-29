using Microsoft.EntityFrameworkCore;
using VoenkomatSpo.Infra;
using VoenkomatSpo.Infra.Documents;

DatabaseConnectionSettings.LoadConnectionString();
var count = await LegacyPdfExporter.ExportAsync();
await using var db = new VoenkomatDbContextFactory().CreateDbContext();
await db.Database.MigrateAsync();
Console.WriteLine($"PDF перенесено в файлы: {count}. Миграции применены.");
