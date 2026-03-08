namespace Domain.Contracts.Infrastructure.Services.BCrypt;

public class BCryptConfiguration
{
    public const string SectionName = "BCrypt";
    public int WorkFactor { get; set; } = default!;
}