using DemoPdf.Api.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace DemoPdf.Api.Application.Reports;

/// <summary>
/// Reporte PDF del listado de usuarios del sistema.
/// </summary>
public class UsersReportDocument : IDocument
{
    private static readonly Color HeaderColor = Color.FromHex("#2C3E50");
    private static readonly Color ZebraColor = Color.FromHex("#F2F5F8");
    private static readonly Color BorderColor = Color.FromHex("#D6DDE5");
    private static readonly Color MutedColor = Color.FromHex("#6B7A8C");
    private static readonly Color ActiveColor = Color.FromHex("#1E8449");
    private static readonly Color InactiveColor = Color.FromHex("#C0392B");

    private readonly IReadOnlyList<UserDto> _users;
    private readonly DateTime _generatedAt = DateTime.Now;

    public UsersReportDocument(IReadOnlyList<UserDto> users)
    {
        _users = users;
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
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
                row.RelativeItem().Text("Listado de Usuarios del Sistema")
                    .FontSize(18).Bold().FontColor(HeaderColor);

                row.ConstantItem(230).AlignRight().AlignBottom()
                    .Text($"Generado: {_generatedAt:dd/MM/yyyy HH:mm}")
                    .FontSize(9).FontColor(MutedColor);
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
                columns.ConstantColumn(40);   // Id
                columns.RelativeColumn(2.2f); // Nombre Completo
                columns.RelativeColumn(1.4f); // Usuario
                columns.RelativeColumn(2f);   // Email
                columns.RelativeColumn(1f);   // Rol
                columns.ConstantColumn(80);   // Estado
            });

            table.Header(header =>
            {
                ComposeHeaderCell(header.Cell(), "Id", HorizontalAlignment.Left);
                ComposeHeaderCell(header.Cell(), "Nombre Completo", HorizontalAlignment.Left);
                ComposeHeaderCell(header.Cell(), "Usuario", HorizontalAlignment.Left);
                ComposeHeaderCell(header.Cell(), "Email", HorizontalAlignment.Left);
                ComposeHeaderCell(header.Cell(), "Rol", HorizontalAlignment.Left);
                ComposeHeaderCell(header.Cell(), "Estado", HorizontalAlignment.Center);
            });

            for (var i = 0; i < _users.Count; i++)
            {
                var user = _users[i];
                var background = i % 2 == 1 ? ZebraColor : Colors.White;

                ComposeBodyCell(table.Cell(), user.Id.ToString(), HorizontalAlignment.Left, background);
                ComposeBodyCell(table.Cell(), user.FullName, HorizontalAlignment.Left, background);
                ComposeBodyCell(table.Cell(), user.Username, HorizontalAlignment.Left, background);
                ComposeBodyCell(table.Cell(), user.Email, HorizontalAlignment.Left, background);
                ComposeBodyCell(table.Cell(), user.Role, HorizontalAlignment.Left, background);

                // Estado con color (Activo/Inactivo)
                table.Cell()
                    .Background(background)
                    .Border(0.5f).BorderColor(BorderColor)
                    .Padding(5)
                    .AlignCenter()
                    .Text(user.IsActive ? "Activo" : "Inactivo")
                    .FontSize(9).SemiBold()
                    .FontColor(user.IsActive ? ActiveColor : InactiveColor);
            }
        });
    }

    private static void ComposeHeaderCell(IContainer container, string text, HorizontalAlignment alignment)
    {
        Align(
            container
                .Background(HeaderColor)
                .BorderRight(1).BorderColor(Colors.White)
                .Padding(6),
            alignment)
            .Text(text)
            .FontColor(Colors.White).SemiBold().FontSize(9.5f);
    }

    private static void ComposeBodyCell(IContainer container, string text, HorizontalAlignment alignment, Color background)
    {
        Align(
            container
                .Background(background)
                .Border(0.5f).BorderColor(BorderColor)
                .Padding(5),
            alignment)
            .Text(text)
            .FontSize(9);
    }

    private static IContainer Align(IContainer container, HorizontalAlignment alignment) => alignment switch
    {
        HorizontalAlignment.Right => container.AlignRight(),
        HorizontalAlignment.Center => container.AlignCenter(),
        _ => container.AlignLeft()
    };

    private void ComposeFooter(IContainer container)
    {
        container.AlignRight().Text(text =>
        {
            text.DefaultTextStyle(x => x.FontSize(9).FontColor(MutedColor));
            text.Span("Página ");
            text.CurrentPageNumber();
            text.Span(" de ");
            text.TotalPages();
        });
    }
}
