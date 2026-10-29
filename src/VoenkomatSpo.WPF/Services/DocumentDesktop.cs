using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using VoenkomatSpo.Core.Documents;

namespace VoenkomatSpo.WPF.Services;

public sealed class DocumentDesktop : IDocumentDesktop
{
    public async Task<bool> SaveAsync(PdfFile file)
    {
        var dialog = new SaveFileDialog { Filter = "Документ PDF (*.pdf)|*.pdf", FileName = file.FileName, DefaultExt = ".pdf", AddExtension = true };
        if (dialog.ShowDialog(Application.Current.MainWindow) != true) return false;
        await File.WriteAllBytesAsync(dialog.FileName, file.Bytes);
        return true;
    }

    public async Task OpenAsync(PdfFile file, bool print = false)
    {
        if (print)
        {
            var dialog = new PrintDialog();
            if (dialog.ShowDialog() != true) return;
            var document = await PdfPrintDocument.CreateAsync(file.Bytes, new Size(dialog.PrintableAreaWidth, dialog.PrintableAreaHeight));
            dialog.PrintDocument(document.DocumentPaginator, file.FileName);
            return;
        }
        var folder = Path.Combine(Path.GetTempPath(), "VoenkomatSpo", "Preview");
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, $"{Guid.NewGuid():N}_{Path.GetFileName(file.FileName)}");
        await File.WriteAllBytesAsync(path, file.Bytes);
        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true, Verb = "open" });
    }

    public bool Confirm(string message) => MessageBox.Show(Application.Current.MainWindow, message, "Подтверждение",
        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

    public void ShowInformation(string title, string message) => MessageBox.Show(Application.Current.MainWindow, message, title,
        MessageBoxButton.OK, MessageBoxImage.Information);
}
