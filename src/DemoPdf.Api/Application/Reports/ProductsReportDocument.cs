using System.Globalization;
using DemoPdf.Api.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DemoPdf.Api.Application.Reports;

/// <summary>
/// Reporte PDF del catálogo / listado de productos.
/// </summary>
public class ProductsReportDocument : IDocument
{
    private static readonly Color HeaderColor = Color.FromHex("#34495E");
    private static readonly Color ZebraColor = Color.FromHex("#F2F5F8");
    private static readonly Color BorderColor = Color.FromHex("#D6DDE5");
    private static readonly Color MutedColor = Color.FromHex("#6B7A8C");
    private static readonly CultureInfo MoneyCulture = new("es-PY");

    private readonly IReadOnlyList<ProductDto> _products;
    private readonly string? _warehouseLabel;
    private readonly DateTime _issuedAt = DateTime.Now;

    public ProductsReportDocument(IReadOnlyList<ProductDto> products, string? warehouseLabel)
    {
        _products = products;
        _warehouseLabel = warehouseLabel;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(28);
            page.DefaultTextStyle(x => x.FontSize(9.5f));

            ComposeHeader(page.Header());
            ComposeTable(page.Content().PaddingVertical(10));
            ComposeFooter(page.Footer());
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Catálogo / Listado de Productos")
                        .FontSize(20).Bold().FontColor(HeaderColor);
                    col.Item().PaddingTop(2).Text($"Depósito: {_warehouseLabel ?? "Todos los depósitos"}")
                        .FontSize(10).FontColor(MutedColor);
                });

                row.ConstantItem(230).AlignRight().Column(col =>
                {
                    col.Item().AlignRight().Text($"Emitido: {_issuedAt:dd/MM/yyyy}")
                        .FontSize(9).FontColor(MutedColor);
                    col.Item().AlignRight().Text($"Hora: {_issuedAt:HH:mm}")
                        .FontSize(9).FontColor(MutedColor);
                    col.Item().AlignRight().Text($"Cantidad de productos: {_products.Count}")
                        .FontSize(9).FontColor(MutedColor);
                });
            });

            column.Item().PaddingTop(6).Height(2).Background(HeaderColor);
        });
    }

    private void ComposeTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(80);   // SKU
                columns.RelativeColumn(2.4f); // Nombre
                columns.RelativeColumn(1.8f); // Depósito
                columns.ConstantColumn(100);  // Precio
                columns.ConstantColumn(60);   // Stock
                columns.ConstantColumn(120);  // Total Valorizado
            });

            // Encabezado
            table.Header(header =>
            {
                ComposeHeaderCell(header.Cell(), "SKU", HorizontalAlignment.Left);
                ComposeHeaderCell(header.Cell(), "Nombre", HorizontalAlignment.Left);
                ComposeHeaderCell(header.Cell(), "Depósito", HorizontalAlignment.Left);
                ComposeHeaderCell(header.Cell(), "Precio", HorizontalAlignment.Right);
                ComposeHeaderCell(header.Cell(), "Stock", HorizontalAlignment.Right);
                ComposeHeaderCell(header.Cell(), "Total Valorizado", HorizontalAlignment.Right);
            });

            // Filas con zebra striping
            for (var i = 0; i < _products.Count; i++)
            {
                var product = _products[i];
                var background = i % 2 == 1 ? ZebraColor : Colors.White;

                ComposeBodyCell(table.Cell(), product.Sku, HorizontalAlignment.Left, background);
                ComposeBodyCell(table.Cell(), product.Name, HorizontalAlignment.Left, background);
                ComposeBodyCell(table.Cell(), product.WarehouseName ?? "-", HorizontalAlignment.Left, background);
                ComposeBodyCell(table.Cell(), product.Price.ToString("N2", MoneyCulture), HorizontalAlignment.Right, background);
                ComposeBodyCell(table.Cell(), product.Stock.ToString("N0", MoneyCulture), HorizontalAlignment.Right, background);
                ComposeBodyCell(table.Cell(), product.TotalValue.ToString("N2", MoneyCulture), HorizontalAlignment.Right, background, bold: true);
            }

            // Fila de totales
            var totalStock = _products.Sum(p => p.Stock);
            var totalValue = _products.Sum(p => p.TotalValue);

            table.Cell().ColumnSpan(4).Background(HeaderColor).Padding(6)
                .AlignRight().Text("TOTALES").FontColor(Colors.White).SemiBold();
            table.Cell().Background(HeaderColor).Padding(6)
                .AlignRight().Text(totalStock.ToString("N0", MoneyCulture)).FontColor(Colors.White).SemiBold();
            table.Cell().Background(HeaderColor).Padding(6)
                .AlignRight().Text(totalValue.ToString("N2", MoneyCulture)).FontColor(Colors.White).SemiBold();
        });
    }

    private static void ComposeHeaderCell(IContainer container, string text, HorizontalAlignment alignment)
    {
        var aligned = Align(
            container
                .Background(HeaderColor)
                .BorderRight(1).BorderColor(Colors.White)
                .Padding(6),
            alignment);

        aligned
            .Text(text)
            .FontColor(Colors.White).SemiBold().FontSize(9.5f);
    }

    private static void ComposeBodyCell(IContainer container, string text, HorizontalAlignment alignment, Color background, bool bold = false)
    {
        var aligned = Align(
            container
                .Background(background)
                .Border(0.5f).BorderColor(BorderColor)
                .Padding(5),
            alignment);

        var textDescriptor = aligned
            .Text(text)
            .FontSize(9);

        if (bold) textDescriptor.SemiBold();
    }

    private static IContainer Align(IContainer container, HorizontalAlignment alignment) => alignment switch
    {
        HorizontalAlignment.Right => container.AlignRight(),
        HorizontalAlignment.Center => container.AlignCenter(),
        _ => container.AlignLeft()
    };

    private void ComposeFooter(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Text("Documento confidencial — uso interno. Generado por DemoPdf.Api (QuestPDF).")
                .FontSize(8).FontColor(MutedColor);

            row.ConstantItem(120).AlignRight().Text(text =>
            {
                text.DefaultTextStyle(x => x.FontSize(9).FontColor(MutedColor));
                text.Span("Página ");
                text.CurrentPageNumber();
                text.Span(" de ");
                text.TotalPages();
            });
        });
    }
}
