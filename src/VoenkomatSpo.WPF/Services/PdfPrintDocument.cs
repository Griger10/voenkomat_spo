using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Data.Pdf;
using Windows.Storage.Streams;

namespace VoenkomatSpo.WPF.Services;

public static class PdfPrintDocument
{
    public static async Task<FixedDocument> CreateAsync(byte[] bytes, Size pageSize)
    {
        using var input = new InMemoryRandomAccessStream();
        using (var writer = new DataWriter(input))
        {
            writer.WriteBytes(bytes);
            await writer.StoreAsync();
            writer.DetachStream();
        }
        input.Seek(0);
        var pdf = await PdfDocument.LoadFromStreamAsync(input);
        var result = new FixedDocument();
        result.DocumentPaginator.PageSize = pageSize;
        for (uint i = 0; i < pdf.PageCount; i++)
        {
            using var source = pdf.GetPage(i);
            using var imageStream = new InMemoryRandomAccessStream();
            await source.RenderToStreamAsync(imageStream, new PdfPageRenderOptions
            {
                DestinationWidth = (uint)Math.Ceiling(source.Size.Width * 2),
                DestinationHeight = (uint)Math.Ceiling(source.Size.Height * 2)
            });
            using var reader = new DataReader(imageStream.GetInputStreamAt(0));
            await reader.LoadAsync((uint)imageStream.Size);
            var imageBytes = new byte[(int)imageStream.Size];
            reader.ReadBytes(imageBytes);
            using var memory = new MemoryStream(imageBytes);
            var bitmap = new BitmapImage();
            bitmap.BeginInit(); bitmap.CacheOption = BitmapCacheOption.OnLoad; bitmap.StreamSource = memory; bitmap.EndInit(); bitmap.Freeze();
            var page = new FixedPage { Width = pageSize.Width, Height = pageSize.Height };
            page.Children.Add(new Image { Source = bitmap, Width = pageSize.Width, Height = pageSize.Height, Stretch = Stretch.Uniform });
            result.Pages.Add(new PageContent { Child = page });
        }
        return result;
    }
}
