namespace VoenkomatSpo.Infra.Documents;

public static class DocumentFiles
{
    public static string DefaultDirectory => string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOCUMENTS_PATH"))
        ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VoenkomatSpo", "Documents")
        : Environment.GetEnvironmentVariable("DOCUMENTS_PATH")!;

    public static string Resolve(string directory, string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName) || Path.GetFileName(fileName) != fileName || !fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Некорректный путь документа.");
        return Path.Combine(Path.GetFullPath(directory), fileName);
    }
}
