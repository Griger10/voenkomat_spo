using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using VoenkomatSpo.Core.Documents;

namespace VoenkomatSpo.Infra.Documents;

public sealed class DocumentPdfRenderer : IPdfRenderer
{
    private static readonly object FontLock = new();

    public byte[] Render(PdfContent content)
    {
        lock (FontLock)
            GlobalFontSettings.FontResolver ??= new EmbeddedFontResolver();

        var document = new Document();
        document.Info.Title = content.Title;
        document.Info.Author = "Воинский учёт СПО";
        var normal = document.Styles[StyleNames.Normal]!;
        normal.Font.Name = "Inter";
        normal.Font.Size = 9.5;
        normal.ParagraphFormat.SpaceAfter = Unit.FromPoint(7);
        var section = document.AddSection();
        section.PageSetup.PageFormat = PageFormat.A4;
        section.PageSetup.TopMargin = Unit.FromCentimeter(1.8);
        section.PageSetup.BottomMargin = Unit.FromCentimeter(1.8);
        section.PageSetup.LeftMargin = Unit.FromCentimeter(1.8);
        section.PageSetup.RightMargin = Unit.FromCentimeter(1.8);
        var header = section.AddParagraph("ВОИНСКИЙ УЧЁТ СТУДЕНТОВ СПО");
        header.Format.Font.Color = Color.FromRgb(53, 104, 75);
        header.Format.Font.Size = 9;
        header.Format.SpaceAfter = Unit.FromPoint(16);
        var title = section.AddParagraph(content.Title);
        title.Format.Font.Bold = true;
        title.Format.Font.Size = 21;
        title.Format.Font.Color = Color.FromRgb(23, 61, 42);
        title.Format.SpaceAfter = Unit.FromPoint(9);
        section.AddParagraph(content.Subtitle);
        var note = section.AddParagraph("Базовая информационная выгрузка. Официальный унифицированный бланк не воспроизводится.");
        note.Format.Font.Size = 8;
        note.Format.Font.Color = Colors.DimGray;
        foreach (var part in content.Sections)
        {
            var heading = section.AddParagraph(part.Heading);
            heading.Format.Font.Bold = true;
            heading.Format.SpaceBefore = Unit.FromPoint(14);
            heading.Format.KeepWithNext = true;
            if (part.Fields.Count == 0)
            {
                section.AddParagraph("Нет сведений за выбранный период.");
                continue;
            }
            var table = section.AddTable();
            table.Borders.Color = Color.FromRgb(218, 225, 218);
            table.Borders.Width = 0.5;
            table.AddColumn(Unit.FromCentimeter(5.2));
            table.AddColumn(Unit.FromCentimeter(12.2));
            var rowIndex = 0;
            foreach (var field in part.Fields)
            {
                var row = table.AddRow();
                row.KeepWith = part.Fields.Count - ++rowIndex;
                row.TopPadding = Unit.FromPoint(4);
                row.BottomPadding = Unit.FromPoint(4);
                row.Cells[0].Shading.Color = Color.FromRgb(242, 244, 241);
                row.Cells[0].AddParagraph(field.Key).Format.SpaceAfter = 0;
                row.Cells[1].AddParagraph(string.IsNullOrWhiteSpace(field.Value) ? "Не указано" : field.Value).Format.SpaceAfter = 0;
            }
        }
        var footer = section.Footers.Primary.AddParagraph();
        footer.Format.Font.Size = 8;
        footer.Format.Alignment = ParagraphAlignment.Right;
        footer.AddText("Воинский учёт СПО • Страница ");
        footer.AddPageField();
        footer.AddText(" из ");
        footer.AddNumPagesField();
        var renderer = new PdfDocumentRenderer { Document = document };
        renderer.RenderDocument();
        using var stream = new MemoryStream();
        renderer.PdfDocument.Save(stream, closeStream: false);
        return stream.ToArray();
    }

    private sealed class EmbeddedFontResolver : IFontResolver
    {
        public FontResolverInfo ResolveTypeface(string familyName, bool bold, bool italic) => new(bold ? "Inter-SemiBold" : "Inter-Regular");
        public byte[] GetFont(string faceName)
        {
            using var stream = typeof(DocumentPdfRenderer).Assembly.GetManifestResourceStream($"VoenkomatSpo.Infra.Documents.Fonts.{faceName}.ttf")
                ?? throw new InvalidOperationException("Не найден встроенный шрифт PDF.");
            using var memory = new MemoryStream();
            stream.CopyTo(memory);
            return memory.ToArray();
        }
    }
}
