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

    private const string LabelColor = "#9CA3AF";
    private const string TextColor = "#111827";
    private const string AccentColor = "#374151";
    private const string DividerColor = "#E5E7EB";
    private const string LightBg = "#F9FAFB";
    private const string LogoUrl = "https://res.cloudinary.com/dlg9nca2v/image/upload/v1773634599/logo-negro-manifesto_xv1arl.png";

    private readonly FiscalDocumentConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;
    private static byte[]? _cachedLogo;

    public InvoicePdfService(
        IOptions<FiscalDocumentConfiguration> config,
        IHttpClientFactory httpClientFactory)
    {
        _config = config.Value;
        _httpClientFactory = httpClientFactory;
    }

    private async Task<byte[]?> GetLogoAsync()
    {
        if (_cachedLogo is not null) return _cachedLogo;

        try
        {
            var client = _httpClientFactory.CreateClient();
            _cachedLogo = await client.GetByteArrayAsync(LogoUrl);
            return _cachedLogo;
        }
        catch
        {
            return null;
        }
    }

    public async Task<byte[]> GeneratePdfAsync(Invoice invoice, string? serieAdmin = null, long? numeroAdmin = null)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var logoBytes = await GetLogoAsync();

        var documentTypeName = GetDocumentTypeName(invoice);
        var currencyCode = invoice.Currency?.Code ?? "GTQ";
        var currencySymbol = currencyCode == "USD" ? "US$" : "Q";

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.Letter);
                page.MarginHorizontal(50);
                page.MarginVertical(40);
                page.DefaultTextStyle(x => x.FontSize(9).FontColor(TextColor).FontFamily("Helvetica"));

                page.Content().Column(col =>
                {
                    // ── HEADER: Logo area + Document type ──
                    ComposeHeader(col, invoice, documentTypeName, logoBytes);

                    col.Item().PaddingTop(20);

                    // ── COMPANY INFO ──
                    ComposeCompanyInfo(col);

                    col.Item().PaddingTop(16);

                    // ── BUYER INFO + DATES (two columns) ──
                    ComposeBuyerAndDates(col, invoice, currencyCode);

                    col.Item().PaddingTop(12);

                    // ── FISCAL DATA ──
                    ComposeFiscalData(col, invoice);

                    col.Item().PaddingTop(20);

                    // ── DETALLE ──
                    ComposeDetalle(col, invoice, currencySymbol);

                    col.Item().PaddingTop(8);

                    // ── TOTALES ──
                    ComposeTotales(col, invoice, currencySymbol, currencyCode);

                    col.Item().PaddingTop(16);

                    // ── TOTAL EN LETRAS ──
                    ComposeTotalEnLetras(col, invoice, currencyCode);

                    col.Item().PaddingTop(8);

                    // ── ISR ──
                    col.Item().Text("Sujeto a retenci\u00f3n definitiva ISR")
                        .FontSize(8).FontColor(LabelColor).AlignCenter();

                    // ── FACTURA ASOCIADA (credit notes) ──
                    if (invoice.InvoiceType == InvoiceType.CreditNote && invoice.OriginalInvoice is not null)
                    {
                        var orig = invoice.OriginalInvoice;
                        var origDateUtc = DateTime.SpecifyKind(orig.Date, DateTimeKind.Utc);
                        var origFechaLocal = TimeZoneInfo.ConvertTimeFromUtc(origDateUtc, GuatemalaTimezone);

                        col.Item().PaddingTop(6)
                            .Text($"Factura Asociada Serie {orig.FiscalSerie}  Numero {orig.FiscalNumero}  Fecha {origFechaLocal:dd/MM/yyyy}")
                            .FontSize(8).FontColor(AccentColor).AlignCenter();
                    }

                    col.Item().PaddingTop(16);

                    // ── CERTIFICADOR ──
                    ComposeCertificador(col);

                    // ── DATOS ADICIONALES ──
                    ComposeDatosAdicionales(col, serieAdmin, numeroAdmin);
                });

                // ── FOOTER ──
                page.Footer().Row(footer =>
                {
                    footer.ConstantItem(80).AlignLeft().AlignMiddle().Column(c =>
                    {
                        if (logoBytes is not null)
                            c.Item().Height(20).Image(logoBytes).FitHeight();
                        else
                            c.Item().Text("manifesto").FontSize(10).Bold().FontColor(TextColor);
                    });

                    footer.RelativeItem().AlignRight().Text(text =>
                    {
                        text.DefaultTextStyle(x => x.FontSize(7).FontColor(LabelColor));
                        text.Span("Documento Tributario Electr\u00f3nico (DTE) \u2022 FEL Guatemala \u2022 P\u00e1gina ");
                        text.CurrentPageNumber();
                        text.Span(" de ");
                        text.TotalPages();
                    });
                });
            });
        });

        return await Task.FromResult(document.GeneratePdf());
    }

    // ──────────────────────────────────────────────
    //  HEADER
    // ──────────────────────────────────────────────
    private static void ComposeHeader(ColumnDescriptor col, Invoice invoice, string documentTypeName, byte[]? logoBytes)
    {
        col.Item().Row(row =>
        {
            // Left: logo
            row.RelativeItem().AlignLeft().AlignBottom().Column(logo =>
            {
                if (logoBytes is not null)
                    logo.Item().Height(60).Image(logoBytes).FitHeight();
                else
                    logo.Item().Text("M.").FontSize(48).Bold().FontColor(TextColor);
            });

            // Right: document type + number
            row.RelativeItem().AlignRight().AlignBottom().Column(right =>
            {
                right.Item().AlignRight()
                    .Text(documentTypeName)
                    .FontSize(28).Bold().FontColor(TextColor);

                right.Item().AlignRight().PaddingTop(2)
                    .Text($"#{invoice.InvoiceNumber}")
                    .FontSize(11).FontColor(LabelColor);
            });
        });
    }

    // ──────────────────────────────────────────────
    //  COMPANY INFO
    // ──────────────────────────────────────────────
    private void ComposeCompanyInfo(ColumnDescriptor col)
    {
        col.Item().Column(company =>
        {
            company.Item().Text(_config.NombreComercial)
                .FontSize(14).Bold().FontColor(TextColor);

            company.Item().PaddingTop(2)
                .Text($"NIT  {_config.Nit}")
                .FontSize(9).FontColor(LabelColor);

            company.Item().PaddingTop(1)
                .Text(_config.DireccionEmisor)
                .FontSize(9).FontColor(LabelColor);
        });
    }

    // ──────────────────────────────────────────────
    //  BUYER INFO + DATES
    // ──────────────────────────────────────────────
    private static void ComposeBuyerAndDates(ColumnDescriptor col, Invoice invoice, string currencyCode)
    {
        var dateUtc = DateTime.SpecifyKind(invoice.Date, DateTimeKind.Utc);
        var fechaLocal = TimeZoneInfo.ConvertTimeFromUtc(dateUtc, GuatemalaTimezone);

        col.Item().Row(row =>
        {
            // Left column: dates + currency
            row.RelativeItem().Column(left =>
            {
                LabelValue(left, "Fecha de Emisi\u00f3n", fechaLocal.ToString("dd/MM/yyyy"));

                if (invoice.DueDate.HasValue)
                {
                    var dueDateUtc = DateTime.SpecifyKind(invoice.DueDate.Value, DateTimeKind.Utc);
                    var dueDateLocal = TimeZoneInfo.ConvertTimeFromUtc(dueDateUtc, GuatemalaTimezone);
                    LabelValue(left, "Fecha de Vencimiento", dueDateLocal.ToString("dd/MM/yyyy"));
                }

                LabelValue(left, "Moneda", currencyCode);

                if (currencyCode != "GTQ" && invoice.ExchangeRate != 1)
                {
                    LabelValue(left, "Tasa de Cambio", invoice.ExchangeRate.ToString("N2"));
                }
            });

            row.ConstantItem(20);

            // Right column: buyer
            row.RelativeItem().Column(right =>
            {
                LabelValue(right, "NIT", invoice.Nit);

                var displayName = invoice.Client is not null && !string.IsNullOrWhiteSpace(invoice.Client.LegalName)
                    ? invoice.Client.LegalName
                    : invoice.Name;
                LabelValue(right, "Nombre", displayName);

                if (!string.IsNullOrWhiteSpace(invoice.Address))
                {
                    LabelValue(right, "Direcci\u00f3n", invoice.Address);
                }
            });
        });
    }

    // ──────────────────────────────────────────────
    //  FISCAL DATA
    // ──────────────────────────────────────────────
    private static void ComposeFiscalData(ColumnDescriptor col, Invoice invoice)
    {
        var hasFiscal = !string.IsNullOrWhiteSpace(invoice.FiscalSerie)
                        && !invoice.FiscalSerie.StartsWith("CONTINGENCIA");

        if (!hasFiscal && invoice.FiscalSerie?.StartsWith("CONTINGENCIA") != true)
            return;

        // Contingency banner
        if (invoice.FiscalSerie is not null && invoice.FiscalSerie.StartsWith("CONTINGENCIA"))
        {
            var numAcceso = invoice.FiscalSerie.Replace("CONTINGENCIA-", "");
            col.Item().Background("#FEF3C7").Padding(8).Column(c =>
            {
                c.Item().Text("DOCUMENTO EN CONTINGENCIA")
                    .FontSize(10).Bold().FontColor("#92400E").AlignCenter();
                c.Item().Text($"N\u00famero de Acceso: {numAcceso}")
                    .FontSize(9).FontColor("#92400E").AlignCenter();
            });
            return;
        }

        // Voided banner
        if (invoice.Status == InvoiceStatus.Anulada)
        {
            col.Item().PaddingBottom(8).Background("#FEE2E2").Padding(8)
                .Text("ANULADA")
                .FontSize(20).Bold().FontColor("#DC2626").AlignCenter();
        }

        col.Item().Row(row =>
        {
            row.RelativeItem().Column(left =>
            {
                LabelValue(left, "Serie", invoice.FiscalSerie ?? "-");
                LabelValue(left, "N\u00famero DTE", invoice.FiscalNumero ?? "-");
            });

            row.ConstantItem(20);

            row.RelativeItem().Column(right =>
            {
                LabelValue(right, "N\u00famero de Autorizaci\u00f3n",
                    invoice.FiscalAutorizacion ?? "-");
            });
        });
    }

    // ──────────────────────────────────────────────
    //  DETALLE
    // ──────────────────────────────────────────────
    private static void ComposeDetalle(ColumnDescriptor col, Invoice invoice, string currencySymbol)
    {
        var items = invoice.Items.OrderBy(i => i.LineOrder).ToList();

        col.Item().Column(section =>
        {
            // Section title
            section.Item().Text("Detalle")
                .FontSize(14).Bold().FontColor(TextColor);

            section.Item().PaddingTop(6)
                .LineHorizontal(1).LineColor(DividerColor);

            foreach (var item in items)
            {
                section.Item().PaddingVertical(8).Row(row =>
                {
                    row.RelativeItem().Column(left =>
                    {
                        left.Item().Text(item.Description)
                            .FontSize(9).Bold().FontColor(TextColor);

                        left.Item().PaddingTop(1)
                            .Text($"{item.Quantity:N2} x {currencySymbol} {item.UnitPrice:N2}")
                            .FontSize(8).FontColor(LabelColor);
                    });

                    row.ConstantItem(100).AlignRight().AlignMiddle()
                        .Text($"{currencySymbol} {item.Total:N2}")
                        .FontSize(10).Bold().FontColor(TextColor);
                });

                section.Item().LineHorizontal(0.5f).LineColor(DividerColor);
            }
        });
    }

    // ──────────────────────────────────────────────
    //  TOTALES
    // ──────────────────────────────────────────────
    private static void ComposeTotales(ColumnDescriptor col, Invoice invoice, string currencySymbol, string currencyCode)
    {
        col.Item().Row(row =>
        {
            row.RelativeItem(); // spacer

            row.ConstantItem(250).Column(totals =>
            {
                // Subtotal
                TotalRow(totals, "Subtotal", $"{currencySymbol} {invoice.Subtotal:N2}", false);

                // IVA
                TotalRow(totals, "IVA (12%)", $"{currencySymbol} {invoice.TaxAmount:N2}", false);

                // Divider
                totals.Item().PaddingVertical(4)
                    .LineHorizontal(1).LineColor(DividerColor);

                // Total
                totals.Item().PaddingVertical(2).Row(r =>
                {
                    r.RelativeItem().AlignLeft()
                        .Text("Total")
                        .FontSize(12).Bold().FontColor(TextColor);
                    r.RelativeItem().AlignRight()
                        .Text($"{currencySymbol} {invoice.Total:N2}")
                        .FontSize(14).Bold().FontColor(TextColor);
                });
            });
        });
    }

    // ──────────────────────────────────────────────
    //  TOTAL EN LETRAS
    // ──────────────────────────────────────────────
    private static void ComposeTotalEnLetras(ColumnDescriptor col, Invoice invoice, string currencyCode)
    {
        var currencyName = currencyCode == "USD" ? "D\u00f3lares" : "Quetzales";
        var totalEnLetras = NumberToWords(invoice.Total);

        col.Item().Background(LightBg).Padding(8)
            .Text($"Total en {currencyName}: {totalEnLetras}")
            .FontSize(8).FontColor(AccentColor).AlignCenter();
    }

    // ──────────────────────────────────────────────
    //  CERTIFICADOR
    // ──────────────────────────────────────────────
    private void ComposeCertificador(ColumnDescriptor col)
    {
        if (string.IsNullOrWhiteSpace(_config.NitCertificador)) return;

        col.Item().Column(section =>
        {
            section.Item().LineHorizontal(0.5f).LineColor(DividerColor);

            section.Item().PaddingTop(8).Row(row =>
            {
                row.RelativeItem().Column(left =>
                {
                    left.Item().Text("Certificador").FontSize(7).FontColor(LabelColor);
                    left.Item().Text(_config.NombreCertificador).FontSize(8).FontColor(AccentColor);
                });

                row.ConstantItem(20);

                row.RelativeItem().Column(right =>
                {
                    right.Item().Text("NIT Certificador").FontSize(7).FontColor(LabelColor);
                    right.Item().Text(_config.NitCertificador).FontSize(8).FontColor(AccentColor);
                });
            });
        });
    }

    // ──────────────────────────────────────────────
    //  DATOS ADICIONALES
    // ──────────────────────────────────────────────
    private static void ComposeDatosAdicionales(ColumnDescriptor col, string? serieAdmin, long? numeroAdmin)
    {
        col.Item().PaddingTop(8).Row(row =>
        {
            row.RelativeItem().Column(left =>
            {
                left.Item().Text("Serie Admin").FontSize(7).FontColor(LabelColor);
                left.Item().Text(serieAdmin ?? "A").FontSize(8).FontColor(AccentColor);
            });

            row.ConstantItem(20);

            row.RelativeItem().Column(right =>
            {
                right.Item().Text("Numero Admin").FontSize(7).FontColor(LabelColor);
                right.Item().Text(numeroAdmin?.ToString() ?? "-").FontSize(8).FontColor(AccentColor);
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
            InvoiceType.CreditNote => "NOTA DE CR\u00c9DITO",
            InvoiceType.Payable => "FACTURA ESPECIAL",
            _ => "FACTURA"
        };
    }

    private static void LabelValue(ColumnDescriptor col, string label, string value)
    {
        col.Item().PaddingBottom(8).Column(c =>
        {
            c.Item().Text(label).FontSize(7).FontColor(LabelColor);
            c.Item().Text(value).FontSize(9).Bold().FontColor(TextColor);
        });
    }

    private static void TotalRow(ColumnDescriptor col, string label, string value, bool isBold)
    {
        col.Item().PaddingVertical(2).Row(row =>
        {
            var labelText = row.RelativeItem().AlignLeft().Text(label).FontSize(9);
            var valueText = row.RelativeItem().AlignRight().Text(value).FontSize(9);

            if (isBold)
            {
                labelText.Bold().FontColor(TextColor);
                valueText.Bold().FontColor(TextColor);
            }
            else
            {
                labelText.FontColor(LabelColor);
                valueText.FontColor(LabelColor);
            }
        });
    }

    // ──────────────────────────────────────────────
    //  NUMBER TO WORDS (Spanish)
    // ──────────────────────────────────────────────
    private static string NumberToWords(decimal amount)
    {
        var intPart = (long)Math.Floor(Math.Abs(amount));
        var centavos = (int)Math.Round((Math.Abs(amount) - intPart) * 100);

        var words = IntToWords(intPart).ToUpper();
        return $"{words} CON {centavos:D2}/100";
    }

    private static string IntToWords(long n)
    {
        if (n == 0) return "cero";
        if (n < 0) return "menos " + IntToWords(-n);

        var parts = new List<string>();

        if (n >= 1_000_000)
        {
            var millions = n / 1_000_000;
            parts.Add(millions == 1 ? "un mill\u00f3n" : IntToWords(millions) + " millones");
            n %= 1_000_000;
        }

        if (n >= 1000)
        {
            var thousands = n / 1000;
            parts.Add(thousands == 1 ? "mil" : IntToWords(thousands) + " mil");
            n %= 1000;
        }

        if (n >= 100)
        {
            if (n == 100)
            {
                parts.Add("cien");
                n = 0;
            }
            else
            {
                parts.Add(((int)(n / 100)) switch
                {
                    1 => "ciento", 2 => "doscientos", 3 => "trescientos",
                    4 => "cuatrocientos", 5 => "quinientos", 6 => "seiscientos",
                    7 => "setecientos", 8 => "ochocientos", 9 => "novecientos", _ => ""
                });
                n %= 100;
            }
        }

        if (n >= 30)
        {
            parts.Add(((int)(n / 10)) switch
            {
                3 => "treinta", 4 => "cuarenta", 5 => "cincuenta",
                6 => "sesenta", 7 => "setenta", 8 => "ochenta", 9 => "noventa", _ => ""
            });
            n %= 10;
            if (n > 0) parts.Add("y");
        }

        if (n >= 20)
        {
            var r = n - 20;
            parts.Add(r == 0 ? "veinte" : "veinti" + UnitsWord(r));
            n = 0;
        }

        if (n >= 10)
        {
            parts.Add(n switch
            {
                10 => "diez", 11 => "once", 12 => "doce", 13 => "trece",
                14 => "catorce", 15 => "quince", 16 => "diecis\u00e9is",
                17 => "diecisiete", 18 => "dieciocho", 19 => "diecinueve", _ => ""
            });
            n = 0;
        }

        if (n > 0) parts.Add(UnitsWord(n));

        return string.Join(" ", parts);
    }

    private static string UnitsWord(long n) => n switch
    {
        1 => "uno", 2 => "dos", 3 => "tres", 4 => "cuatro", 5 => "cinco",
        6 => "seis", 7 => "siete", 8 => "ocho", 9 => "nueve", _ => ""
    };
}
