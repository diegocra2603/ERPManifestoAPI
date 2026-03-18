namespace Domain.Entities.Accounting.Enums;

public enum InvoiceType
{
    Receivable = 1,  // Cuentas por Cobrar (ventas)
    Payable = 2,     // Cuentas por Pagar (compras)
    CreditNote = 3   // Nota de Crédito (ajuste sobre factura emitida)
}
