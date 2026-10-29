using VoenkomatSpo.Core.Documents;

namespace VoenkomatSpo.WPF.Services;

public interface IDocumentDesktop
{
    Task<bool> SaveAsync(PdfFile file);
    Task OpenAsync(PdfFile file, bool print = false);
    bool Confirm(string message);
    void ShowInformation(string title, string message);
}
