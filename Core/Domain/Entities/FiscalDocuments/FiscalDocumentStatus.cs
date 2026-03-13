namespace Domain.Entities.FiscalDocuments;

public enum FiscalDocumentStatus
{
    Pending = 0,
    Certified = 1,
    Contingency = 2,
    Voided = 3,
    Error = 4
}
