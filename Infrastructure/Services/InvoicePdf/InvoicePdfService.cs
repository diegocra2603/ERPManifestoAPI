using Domain.Contracts.Infrastructure.Services.FiscalDocument;
using Domain.Contracts.Infrastructure.Services.InvoicePdf;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Services.InvoicePdf;

public sealed class InvoicePdfService : IInvoicePdfService
{
    private static readonly TimeZoneInfo GuatemalaTimezone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Guatemala");

    private const string HeaderBg = "#D9D9D9";
    private const string CellBorder = "#000000";
    private const float BorderWidth = 0.5f;

    private readonly FiscalDocumentConfiguration _config;

    public InvoicePdfService(
        IOptions<FiscalDocumentConfiguration> config,
        IHttpClientFactory httpClientFactory)
    {
        _config = config.Value;
    }

    public async Task<byte[]> GeneratePdfAsync(Invoice invoice)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var documentTypeName = GetDocumentTypeName(invoice);
        var currencyCode = invoice.Currency?.Code ?? "GTQ";

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.MarginHorizontal(40);
                page.MarginVertical(30);
                page.DefaultTextStyle(x => x.FontSize(8).FontColor("#000000").FontFamily("Helvetica"));

                page.Content().Column(col =>
                {
                    // === ENCABEZADO DEL DOCUMENTO ===
                    ComposeDocumentHeader(col, invoice, documentTypeName);

                    // === ANULADA BANNER ===
                    if (invoice.Status == InvoiceStatus.Anulada)
                    {
                        col.Item().PaddingVertical(6)
                            .Border(2).BorderColor("#DC2626")
                            .Background("#FEE2E2")
                            .PaddingVertical(8)
                            .Text("ANULADA")
                            .FontSize(28).Bold().FontColor("#DC2626").AlignCenter();
                    }

                    // === DATOS DEL VENDEDOR ===
                    ComposeDatosVendedor(col);

                    // === DATOS DEL COMPRADOR ===
                    ComposeDatosComprador(col, invoice);

                    // === DESCRIPCION DEL DOCUMENTO ===
                    ComposeDescripcionDocumento(col, invoice, currencyCode);

                    // === TOTAL EN QUETZALES ===
                    ComposeTotalEnLetras(col, invoice, currencyCode);

                    // === ISR (solo para facturas) ===
                    if (_config.SujetoIsrTrimestral && documentTypeName == "FACTURA")
                    {
                        col.Item().PaddingTop(8)
                            .Text("Sujeto a pagos trimestrales ISR")
                            .FontSize(9).Bold().FontColor("#DC2626").AlignCenter();
                    }

                    // === FACTURA ASOCIADA (para notas de credito) ===
                    if (documentTypeName == "NOTA DE CREDITO" && invoice.OriginalInvoice is not null)
                    {
                        var orig = invoice.OriginalInvoice;
                        var origDateUtc = DateTime.SpecifyKind(orig.Date, DateTimeKind.Utc);
                        var origFechaLocal = TimeZoneInfo.ConvertTimeFromUtc(origDateUtc, GuatemalaTimezone);
                        var associatedText = $"Factura Asociada Serie {orig.FiscalSerie}  Numero {orig.FiscalNumero}  Fecha {origFechaLocal:dd/MM/yyyy}";

                        col.Item().PaddingTop(6)
                            .Border(1).BorderColor("#DC2626")
                            .PaddingVertical(4).PaddingHorizontal(8)
                            .Text(associatedText)
                            .FontSize(8).Bold().FontColor("#000000").AlignCenter();
                    }

                    // === DATOS DEL CERTIFICADOR ===
                    ComposeDatosCertificador(col);

                    // === DATOS ADICIONALES ===
                    ComposeDatosAdicionales(col, invoice);
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(7).FontColor("#888888"));
                    text.Span("Documento Tributario Electr\u00f3nico (DTE) \u2022 FEL Guatemala \u2022 P\u00e1gina ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            });
        });

        return await Task.FromResult(document.GeneratePdf());
    }

    // ──────────────────────────────────────────────
    //  ENCABEZADO
    // ──────────────────────────────────────────────
    private void ComposeDocumentHeader(ColumnDescriptor col, Invoice invoice, string documentTypeName)
    {
        col.Item().Border(BorderWidth).BorderColor(CellBorder).Column(header =>
        {
            // DOCUMENTO TRIBUTARIO ELECTRONICO
            header.Item()
                .Background(HeaderBg)
                .PaddingVertical(4)
                .Text("DOCUMENTO TRIBUTARIO ELECTRONICO")
                .FontSize(9).Bold().AlignCenter();

            // Tipo de documento + Serie y Numero
            header.Item().PaddingVertical(6).Column(info =>
            {
                info.Item().Text(documentTypeName)
                    .FontSize(16).Bold().AlignCenter();

                var hasFiscal = !string.IsNullOrWhiteSpace(invoice.FiscalSerie)
                                && !invoice.FiscalSerie.StartsWith("CONTINGENCIA");

                if (hasFiscal)
                {
                    info.Item().PaddingTop(2)
                        .Text($"Serie: {invoice.FiscalSerie}  Numero: {invoice.FiscalNumero}")
                        .FontSize(9).AlignCenter();

                    info.Item().PaddingTop(4).PaddingHorizontal(20).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Numero de Autorizaci\u00f3n:")
                                .FontSize(7).Bold().FontColor("#DC2626");
                            c.Item().Text(invoice.FiscalAutorizacion ?? "-")
                                .FontSize(7);
                        });
                    });
                }

                // Contingencia
                if (invoice.FiscalSerie is not null && invoice.FiscalSerie.StartsWith("CONTINGENCIA"))
                {
                    var numAcceso = invoice.FiscalSerie.Replace("CONTINGENCIA-", "");
                    info.Item().PaddingTop(4)
                        .Text("DOCUMENTO EN CONTINGENCIA")
                        .FontSize(10).Bold().FontColor("#92400E").AlignCenter();
                    info.Item().Text($"N\u00famero de Acceso: {numAcceso}")
                        .FontSize(9).Bold().FontColor("#92400E").AlignCenter();
                }

                // Fecha de Emisión
                var dateUtc = DateTime.SpecifyKind(invoice.Date, DateTimeKind.Utc);
                var fechaLocal = TimeZoneInfo.ConvertTimeFromUtc(dateUtc, GuatemalaTimezone);

                info.Item().PaddingTop(4).PaddingHorizontal(20).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Fecha de Emisi\u00f3n:")
                            .FontSize(7).Bold().FontColor("#DC2626");
                        c.Item().Text(fechaLocal.ToString("dd/MM/yyyy"))
                            .FontSize(8);
                    });
                });
            });
        });
    }

    // ──────────────────────────────────────────────
    //  DATOS DEL VENDEDOR
    // ──────────────────────────────────────────────
    private void ComposeDatosVendedor(ColumnDescriptor col)
    {
        col.Item().Border(BorderWidth).BorderColor(CellBorder).Column(section =>
        {
            // Section header
            section.Item()
                .Background(HeaderBg)
                .PaddingVertical(3)
                .Text("DATOS DEL VENDEDOR")
                .FontSize(8).Bold().AlignCenter();

            // NIT y Nombre
            section.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(150);
                    columns.RelativeColumn();
                });

                // Row: NIT + Nombre
                TableCell(table, "NIT del contribuyente", true);
                TableCell(table, "Nombre, raz\u00f3n o denominaci\u00f3n social del contribuyente", true);

                TableCell(table, _config.Nit);
                TableCell(table, _config.NombreComercial);
            });

            // Nombre comercial
            section.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(150);
                    columns.RelativeColumn();
                });

                TableCell(table, "NOMBRE DE ESTABLECIMIENTO\nCOMERCIAL", true);
                TableCell(table, _config.NombreComercial);
            });

            // Dirección
            section.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(150);
                    columns.RelativeColumn();
                });

                TableCell(table, "DIRECCION", true);
                TableCell(table, _config.DireccionEmisor);
            });
        });
    }

    // ──────────────────────────────────────────────
    //  DATOS DEL COMPRADOR
    // ──────────────────────────────────────────────
    private static void ComposeDatosComprador(ColumnDescriptor col, Invoice invoice)
    {
        col.Item().Border(BorderWidth).BorderColor(CellBorder).Column(section =>
        {
            // Section header
            section.Item()
                .Background(HeaderBg)
                .PaddingVertical(3)
                .Text("DATOS DEL COMPRADOR")
                .FontSize(8).Bold().AlignCenter();

            section.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(150);
                    columns.RelativeColumn();
                });

                // Headers
                TableCell(table, "NIT", true);
                TableCell(table, "Nombre, raz\u00f3n o denominaci\u00f3n social del contribuyente", true);

                // Values
                TableCell(table, invoice.Nit);

                var displayName = invoice.Client is not null && !string.IsNullOrWhiteSpace(invoice.Client.LegalName)
                    ? invoice.Client.LegalName
                    : invoice.Name;
                TableCell(table, displayName);
            });

            // Dirección del comprador (si existe)
            if (!string.IsNullOrWhiteSpace(invoice.Address))
            {
                section.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(150);
                        columns.RelativeColumn();
                    });

                    TableCell(table, "DIRECCION", true);
                    TableCell(table, invoice.Address);
                });
            }
        });
    }

    // ──────────────────────────────────────────────
    //  DESCRIPCION DEL DOCUMENTO
    // ──────────────────────────────────────────────
    private static void ComposeDescripcionDocumento(ColumnDescriptor col, Invoice invoice, string currencyCode)
    {
        var symbol = currencyCode == "USD" ? "$" : "Q";
        var items = invoice.Items.OrderBy(i => i.LineOrder).ToList();

        col.Item().Border(BorderWidth).BorderColor(CellBorder).Column(section =>
        {
            // Section header
            section.Item()
                .Background(HeaderBg)
                .Border(BorderWidth).BorderColor(CellBorder)
                .PaddingVertical(3)
                .Text("DESCRIPCION DEL DOCUMENTO")
                .FontSize(8).Bold().AlignCenter();

            // Table header row
            section.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(55);  // CANTIDAD
                    columns.RelativeColumn();     // DESCRIPCION
                    columns.ConstantColumn(65);   // PRECIO UNITARIO
                    columns.ConstantColumn(65);   // SUB-TOTAL
                    columns.ConstantColumn(60);   // DESCUENTO
                    columns.ConstantColumn(70);   // TOTAL
                });

                // Column headers
                TableHeaderCell(table, "CANTIDAD");
                TableHeaderCell(table, "DESCRIPCION");
                TableHeaderCell(table, "PRECIO\nUNITARIO");
                TableHeaderCell(table, "SUB-TOTAL");
                TableHeaderCell(table, "DESCUENTO");
                TableHeaderCell(table, $"TOTAL {symbol}");

                // Items
                foreach (var item in items)
                {
                    TableNumberCell(table, item.Quantity.ToString("N2"));
                    TableCell(table, item.Description);
                    TableNumberCell(table, FormatAmount(item.UnitPrice));
                    TableNumberCell(table, FormatAmount(item.Subtotal));
                    TableNumberCell(table, ""); // Descuento - not implemented yet
                    TableNumberCell(table, FormatAmount(item.Total));
                }

                // Totals row
                TableNumberCell(table, "");
                TableCell(table, "");
                TableNumberCell(table, "");
                TableNumberCell(table, FormatAmount(invoice.Subtotal));
                TableNumberCell(table, "");
                TableNumberCell(table, FormatAmount(invoice.Total));
            });
        });
    }

    // ──────────────────────────────────────────────
    //  TOTAL EN QUETZALES / DOLARES
    // ──────────────────────────────────────────────
    private static void ComposeTotalEnLetras(ColumnDescriptor col, Invoice invoice, string currencyCode)
    {
        var currencyName = currencyCode == "USD" ? "DOLARES" : "QUETZALES";
        var centavos = (int)Math.Round((invoice.Total - Math.Floor(invoice.Total)) * 100);

        col.Item().Border(BorderWidth).BorderColor(CellBorder).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(150);
                columns.RelativeColumn();
            });

            table.Cell()
                .Border(BorderWidth).BorderColor(CellBorder)
                .Padding(4)
                .Text($"TOTAL EN {currencyName}")
                .FontSize(8).Bold();

            table.Cell()
                .Border(BorderWidth).BorderColor(CellBorder)
                .Padding(4)
                .Text($"CON {centavos:D2}/100")
                .FontSize(8).Bold();
        });
    }

    // ──────────────────────────────────────────────
    //  DATOS DEL CERTIFICADOR
    // ──────────────────────────────────────────────
    private void ComposeDatosCertificador(ColumnDescriptor col)
    {
        if (string.IsNullOrWhiteSpace(_config.NitCertificador)) return;

        col.Item().PaddingTop(6).Border(BorderWidth).BorderColor(CellBorder).Column(section =>
        {
            // Section header
            section.Item()
                .Background(HeaderBg)
                .PaddingVertical(3)
                .Text("DATOS DEL CERTIFICADOR")
                .FontSize(8).Bold().AlignCenter();

            section.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(150);
                    columns.RelativeColumn();
                });

                TableCell(table, "NIT del contribuyente", true);
                TableCell(table, "Nombre, raz\u00f3n o denominaci\u00f3n social del contribuyente", true);

                TableCell(table, _config.NitCertificador);
                TableCell(table, _config.NombreCertificador);
            });
        });
    }

    // ──────────────────────────────────────────────
    //  DATOS ADICIONALES
    // ──────────────────────────────────────────────
    private static void ComposeDatosAdicionales(ColumnDescriptor col, Invoice invoice)
    {
        col.Item().PaddingTop(6).Border(BorderWidth).BorderColor(CellBorder).Column(section =>
        {
            // Section header
            section.Item()
                .Background(HeaderBg)
                .PaddingVertical(3)
                .Text("DATOS ADICIONALES")
                .FontSize(8).Bold().AlignCenter();

            section.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(150);
                    columns.RelativeColumn();
                });

                // Headers
                TableCell(table, "Campo", true);
                TableCell(table, "Valor", true);

                // Invoice number
                TableCell(table, "Numero Factura");
                TableCell(table, invoice.InvoiceNumber);

                // Moneda
                TableCell(table, "Moneda");
                TableCell(table, invoice.Currency?.Code ?? "GTQ");

                // Tipo de cambio (si no es 1)
                if (invoice.ExchangeRate != 1)
                {
                    TableCell(table, "Tipo de Cambio");
                    TableCell(table, invoice.ExchangeRate.ToString("N2"));
                }

                // Fecha de vencimiento
                if (invoice.DueDate.HasValue)
                {
                    TableCell(table, "Fecha Vencimiento");
                    TableCell(table, invoice.DueDate.Value.ToString("dd/MM/yyyy"));
                }
            });
        });
    }

    // ──────────────────────────────────────────────
    //  HELPERS
    // ──────────────────────────────────────────────

    private static string GetDocumentTypeName(Invoice invoice)
    {
        return invoice.InvoiceType switch
        {
            InvoiceType.Receivable => "FACTURA",
            InvoiceType.CreditNote => "NOTA DE CREDITO",
            InvoiceType.Payable => "FACTURA ESPECIAL",
            _ => "FACTURA"
        };
    }

    private static void TableCell(TableDescriptor table, string text, bool isHeader = false)
    {
        var cell = table.Cell()
            .Border(BorderWidth).BorderColor(CellBorder)
            .Padding(3);

        if (isHeader)
            cell.Background(HeaderBg).Text(text).FontSize(7).Bold();
        else
            cell.Text(text).FontSize(8);
    }

    private static void TableHeaderCell(TableDescriptor table, string text)
    {
        table.Cell()
            .Border(BorderWidth).BorderColor(CellBorder)
            .Background(HeaderBg)
            .Padding(3)
            .AlignCenter()
            .Text(text).FontSize(7).Bold().AlignCenter();
    }

    private static void TableNumberCell(TableDescriptor table, string text)
    {
        table.Cell()
            .Border(BorderWidth).BorderColor(CellBorder)
            .Padding(3)
            .AlignRight()
            .Text(text).FontSize(8).AlignRight();
    }

    private static string FormatAmount(decimal amount) => amount.ToString("N2");
}
