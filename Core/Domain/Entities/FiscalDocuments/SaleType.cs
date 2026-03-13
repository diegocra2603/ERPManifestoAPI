namespace Domain.Entities.FiscalDocuments;

public enum SaleType
{
    Bienes = 1,
    Servicios = 2
}

public static class SaleTypeExtensions
{
    public static string ToCode(this SaleType saleType) => saleType switch
    {
        SaleType.Bienes => "B",
        SaleType.Servicios => "S",
        _ => "B"
    };

    public static SaleType FromCode(string code) => code.ToUpper() switch
    {
        "B" => SaleType.Bienes,
        "S" => SaleType.Servicios,
        _ => SaleType.Bienes
    };
}
