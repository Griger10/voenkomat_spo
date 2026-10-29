using Npgsql;
using System.Security.Cryptography;

namespace VoenkomatSpo.Infra.Documents;

public static class LegacyPdfExporter
{
    public static async Task<int> ExportAsync(string? directory = null, CancellationToken cancellationToken = default, string? connectionString = null)
    {
        directory ??= DocumentFiles.DefaultDirectory;
        await using var connection = new NpgsqlConnection(connectionString ?? DatabaseConnectionSettings.LoadConnectionString());
        await connection.OpenAsync(cancellationToken);
        await using var check = new NpgsqlCommand("SELECT EXISTS (SELECT 1 FROM information_schema.columns WHERE table_schema='public' AND table_name='GeneratedDocument' AND column_name='pdf_data')", connection);
        if (await check.ExecuteScalarAsync(cancellationToken) is not true) return 0;
        await using (var create = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS \"__DocumentFileExport\" (id integer PRIMARY KEY, filename text NOT NULL)", connection))
            await create.ExecuteNonQueryAsync(cancellationToken);
        var files = new List<(int Id, byte[] Bytes)>();
        await using (var query = new NpgsqlCommand("SELECT id, pdf_data FROM \"GeneratedDocument\"", connection))
        await using (var reader = await query.ExecuteReaderAsync(cancellationToken))
            while (await reader.ReadAsync(cancellationToken)) files.Add((reader.GetInt32(0), (byte[])reader[1]));
        if (files.Count > 0) Directory.CreateDirectory(directory);
        foreach (var file in files)
        {
            var name = $"legacy-{file.Id}-{Convert.ToHexString(SHA256.HashData(file.Bytes))[..16]}.pdf";
            await File.WriteAllBytesAsync(DocumentFiles.Resolve(directory, name), file.Bytes, cancellationToken);
            await using var mark = new NpgsqlCommand("INSERT INTO \"__DocumentFileExport\" (id, filename) VALUES (@id,@file) ON CONFLICT (id) DO UPDATE SET filename=EXCLUDED.filename", connection);
            mark.Parameters.AddWithValue("id", file.Id); mark.Parameters.AddWithValue("file", name);
            await mark.ExecuteNonQueryAsync(cancellationToken);
        }
        return files.Count;
    }
}
