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
    private static readonly string HeaderLogoUrl =
        "https://res.cloudinary.com/dlg9nca2v/image/upload/v1773634599/logo-negro-manifesto_xv1arl.png";

    private static readonly string FooterLogoUrl =
        "https://res.cloudinary.com/dlg9nca2v/image/upload/v1773634597/logo-footer-negro_my78wz.png";

    private static readonly TimeZoneInfo GuatemalaTimezone =
        TimeZoneInfo.FindSystemTimeZoneById("America/Guatemala");

    private readonly FiscalDocumentConfiguration _config;
    private readonly HttpClient _httpClient;

    // Cached logo bytes
    private static byte[]? _headerLogoCache;
    private static byte[]? _footerLogoCache;
    private static readonly SemaphoreSlim _logoLock = new(1, 1);

    public InvoicePdfService(
        IOptions<FiscalDocumentConfiguration> config,
        IHttpClientFactory httpClientFactory)
    {
        _config = config.Value;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<byte[]> GeneratePdfAsync(Invoice invoice)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        await EnsureLogosLoadedAsync();
        var headerLogo = _headerLogoCache;
        var footerLogo = _footerLogoCache;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.MarginHorizontal(50);
                page.MarginVertical(40);
                page.DefaultTextStyle(x => x.FontSize(9).FontColor("#1A1A1A"));

                page.Header().Element(c => ComposeHeader(c, invoice, headerLogo));
                page.Content().Element(c => ComposeContent(c, invoice, invoice.Currency?.Code ?? "GTQ"));
                page.Footer().Element(c => ComposeFooter(c, footerLogo));
            });
        });

        return document.GeneratePdf();
    }

    private async Task EnsureLogosLoadedAsync()
    {
        if (_headerLogoCache is not null && _footerLogoCache is not null) return;

        await _logoLock.WaitAsync();
        try
        {
            if (_headerLogoCache is null)
            {
                try { _headerLogoCache = await _httpClient.GetByteArrayAsync(HeaderLogoUrl); } catch { /* skip */ }
            }

            if (_footerLogoCache is null)
            {
                try { _footerLogoCache = await _httpClient.GetByteArrayAsync(FooterLogoUrl); } catch { /* skip */ }
            }
        }
        finally
        {
            _logoLock.Release();
        }
    }

    private void ComposeHeader(IContainer container, Invoice invoice, byte[]? logo)
    {
        container.Column(col =>
        {
            // Top row: Logo + company info | FACTURA title + number
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    if (logo is not null)
                    {
                        left.Item().Width(120).Image(logo);
                        left.Item().PaddingTop(10);
                    }

                    left.Item().Text(_config.NombreComercial)
                        .FontSize(13).Bold().FontColor("#1A1A1A");
                    left.Item().PaddingTop(6);
                    left.Item().Text($"NIT  {_config.Nit}")
                        .FontSize(8).FontColor("#888888");
                    if (!string.IsNullOrWhiteSpace(_config.DireccionEmisor))
                    {
                        left.Item().Text(_config.DireccionEmisor)
                            .FontSize(8).FontColor("#888888");
                    }
                });

                row.ConstantItem(200).AlignRight().AlignTop().Column(right =>
                {
                    right.Item().Text("FACTURA")
                        .FontSize(28).Bold().FontColor("#1A1A1A").AlignRight();
                    right.Item().PaddingTop(4);
                    right.Item().Text($"#{invoice.InvoiceNumber}")
                        .FontSize(12).FontColor("#888888").AlignRight();
                });
            });

            // Voided banner
            if (invoice.Status == InvoiceStatus.Anulada)
            {
                col.Item().PaddingTop(10).PaddingBottom(4)
                    .BorderTop(1).BorderBottom(1).BorderColor("#DC2626")
                    .PaddingVertical(6)
                    .Text("ANULADA")
                    .FontSize(16).Bold().FontColor("#DC2626").AlignCenter();
            }

            // Contingency banner
            if (invoice.FiscalSerie is not null && invoice.FiscalSerie.StartsWith("CONTINGENCIA"))
            {
                var numAcceso = invoice.FiscalSerie.Replace("CONTINGENCIA-", "");
                col.Item().PaddingTop(10).PaddingBottom(4)
                    .BorderTop(1).BorderBottom(1).BorderColor("#92400E")
                    .PaddingVertical(6).Column(c =>
                    {
                        c.Item().Text("DOCUMENTO EN CONTINGENCIA")
                            .FontSize(11).Bold().FontColor("#92400E").AlignCenter();
                        c.Item().Text($"N\u00famero de Acceso: {numAcceso}")
                            .FontSize(10).Bold().FontColor("#92400E").AlignCenter();
                        c.Item().PaddingTop(4).Text("Utilice este n\u00famero de acceso para consultar el estado de su factura una vez sea certificada.")
                            .FontSize(8).FontColor("#92400E").AlignCenter();
                    });
            }

            col.Item().PaddingTop(20);

            // Two-column info: Emisor details | Receptor details
            col.Item().Row(row =>
            {
                // Left column
                row.RelativeItem().Column(left =>
                {
                    DetailLabel(left, "Fecha de Emisi\u00f3n");
                    var dateUtc = DateTime.SpecifyKind(invoice.Date, DateTimeKind.Utc);
                    var fechaLocal = TimeZoneInfo.ConvertTimeFromUtc(dateUtc, GuatemalaTimezone);
                    left.Item().Text(fechaLocal.ToString("dd/MM/yyyy HH:mm"))
                        .FontSize(9).FontColor("#1A1A1A");
                    left.Item().PaddingTop(6);

                    if (invoice.DueDate.HasValue)
                    {
                        DetailLabel(left, "Fecha de Vencimiento");
                        left.Item().Text(invoice.DueDate.Value.ToString("dd/MM/yyyy"))
                            .FontSize(9).FontColor("#1A1A1A");
                        left.Item().PaddingTop(6);
                    }

                    DetailLabel(left, "Moneda");
                    left.Item().Text(invoice.Currency?.Code ?? "GTQ")
                        .FontSize(9).FontColor("#1A1A1A");

                    if (invoice.ExchangeRate != 1)
                    {
                        left.Item().PaddingTop(6);
                        DetailLabel(left, "Tipo de Cambio");
                        left.Item().Text(invoice.ExchangeRate.ToString("N2"))
                            .FontSize(9).FontColor("#1A1A1A");
                    }
                });

                // Right column - Receptor
                row.RelativeItem().Column(right =>
                {
                    DetailLabel(right, "NIT");
                    right.Item().Text(invoice.Nit)
                        .FontSize(9).FontColor("#1A1A1A");
                    right.Item().PaddingTop(6);

                    var displayName = invoice.Client is not null && !string.IsNullOrWhiteSpace(invoice.Client.LegalName)
                        ? invoice.Client.LegalName
                        : invoice.Name;
                    DetailLabel(right, "Nombre");
                    right.Item().Text(displayName)
                        .FontSize(9).FontColor("#1A1A1A");

                    if (!string.IsNullOrWhiteSpace(invoice.Address))
                    {
                        right.Item().PaddingTop(6);
                        DetailLabel(right, "Direcci\u00f3n");
                        right.Item().Text(invoice.Address)
                            .FontSize(9).FontColor("#1A1A1A");
                    }
                });
            });

            col.Item().PaddingTop(20);

            // Fiscal data - above detail
            var hasFiscal = !string.IsNullOrWhiteSpace(invoice.FiscalSerie)
                            && !invoice.FiscalSerie.StartsWith("CONTINGENCIA");
            if (hasFiscal)
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(fiscal =>
                    {
                        FiscalLine(fiscal, "Serie", invoice.FiscalSerie ?? "-");
                        FiscalLine(fiscal, "N\u00famero DTE", invoice.FiscalNumero ?? "-");
                    });
                    row.RelativeItem().Column(fiscal =>
                    {
                        fiscal.Item().Text("N\u00famero de Autorizaci\u00f3n")
                            .FontSize(7).FontColor("#AAAAAA");
                        fiscal.Item().Text(invoice.FiscalAutorizacion ?? "-")
                            .FontSize(7).FontColor("#1A1A1A");
                    });
                });

                col.Item().PaddingTop(16);
            }

            // Section title
            col.Item().Text("Detalle")
                .FontSize(14).Bold().FontColor("#1A1A1A");
            col.Item().PaddingTop(8);
        });
    }

    private void ComposeContent(IContainer container, Invoice invoice, string currencyCode)
    {
        container.Column(col =>
        {
            // Items - clean list style
            var items = invoice.Items.OrderBy(i => i.LineOrder).ToList();

            // Thin top line
            col.Item().LineHorizontal(0.5f).LineColor("#E0E0E0");

            foreach (var item in items)
            {
                col.Item().PaddingVertical(8).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(item.Description).FontSize(9).FontColor("#1A1A1A");
                        c.Item().Text($"{item.Quantity:N2} x {FormatCurrency(item.UnitPrice, currencyCode)}")
                            .FontSize(7).FontColor("#AAAAAA");
                    });
                    row.ConstantItem(100).AlignRight().AlignMiddle()
                        .Text(FormatCurrency(item.Total, currencyCode)).FontSize(9).FontColor("#1A1A1A");
                });

                col.Item().LineHorizontal(0.5f).LineColor("#E0E0E0");
            }

            col.Item().PaddingTop(16);

            // Totals - right aligned, simple
            col.Item().AlignRight().Width(220).Column(totals =>
            {
                TotalLine(totals, "Subtotal", FormatCurrency(invoice.Subtotal, currencyCode));
                TotalLine(totals, "IVA (12%)", FormatCurrency(invoice.TaxAmount, currencyCode));

                totals.Item().PaddingTop(6).PaddingBottom(6)
                    .LineHorizontal(0.5f).LineColor("#1A1A1A");

                totals.Item().Row(row =>
                {
                    row.RelativeItem().Text("Total")
                        .FontSize(12).Bold().FontColor("#1A1A1A");
                    row.ConstantItem(110).AlignRight()
                        .Text(FormatCurrency(invoice.Total, currencyCode))
                        .FontSize(12).Bold().FontColor("#1A1A1A");
                });
            });

            // Notes
            if (!string.IsNullOrWhiteSpace(invoice.Notes))
            {
                col.Item().PaddingTop(24);
                col.Item().LineHorizontal(0.5f).LineColor("#E0E0E0");
                col.Item().PaddingTop(12);
                col.Item().Text("Notas")
                    .FontSize(10).Bold().FontColor("#1A1A1A");
                col.Item().PaddingTop(4);
                col.Item().Text(invoice.Notes)
                    .FontSize(8).FontColor("#666666").LineHeight(1.4f);
            }

            // Sujeto a pagos trimestrales ISR
            if (_config.SujetoIsrTrimestral)
            {
                col.Item().PaddingTop(16);
                col.Item().Text("Sujeto a pagos trimestrales ISR")
                    .FontSize(8).Bold().FontColor("#1A1A1A").AlignCenter();
            }

            // Certificador data at the bottom
            if (!string.IsNullOrWhiteSpace(_config.NitCertificador))
            {
                col.Item().PaddingTop(24);
                col.Item().LineHorizontal(0.5f).LineColor("#E0E0E0");
                col.Item().PaddingTop(8);
                col.Item().Text("Datos del Certificador")
                    .FontSize(8).Bold().FontColor("#AAAAAA");
                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("NIT Certificador").FontSize(7).FontColor("#AAAAAA");
                        c.Item().Text(_config.NitCertificador).FontSize(8).FontColor("#1A1A1A");
                    });
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Nombre Certificador").FontSize(7).FontColor("#AAAAAA");
                        c.Item().Text(_config.NombreCertificador).FontSize(8).FontColor("#1A1A1A");
                    });
                });
            }
        });
    }

    private static void ComposeFooter(IContainer container, byte[]? footerLogo)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(0.5f).LineColor("#E0E0E0");
            col.Item().PaddingTop(8);

            col.Item().Row(row =>
            {
                if (footerLogo is not null)
                {
                    row.ConstantItem(80).AlignBottom().Image(footerLogo);
                }

                row.RelativeItem().AlignRight().AlignBottom().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(7).FontColor("#AAAAAA"));
                    text.Span("Documento Tributario Electr\u00f3nico (DTE) \u2022 FEL Guatemala \u2022 P\u00e1gina ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            });
        });
    }

    private static void DetailLabel(ColumnDescriptor col, string label)
    {
        col.Item().Text(label).FontSize(7).FontColor("#AAAAAA");
    }

    private static void TotalLine(ColumnDescriptor col, string label, string value)
    {
        col.Item().PaddingVertical(2).Row(row =>
        {
            row.RelativeItem().Text(label).FontSize(9).FontColor("#AAAAAA");
            row.ConstantItem(110).AlignRight()
                .Text(value).FontSize(9).FontColor("#1A1A1A");
        });
    }

    private static void FiscalLine(ColumnDescriptor col, string label, string value)
    {
        col.Item().PaddingVertical(1).Row(row =>
        {
            row.ConstantItem(80).Text(label).FontSize(7).FontColor("#AAAAAA");
            row.RelativeItem().Text(value).FontSize(8).Bold().FontColor("#1A1A1A");
        });
    }

    private static string FormatCurrency(decimal amount, string currencyCode = "GTQ") =>
        currencyCode == "USD" ? $"$ {amount:N2}" : $"Q {amount:N2}";
}
