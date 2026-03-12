using Domain.Entities.FiscalData;
using ErrorOr;

namespace Domain.Contracts.Infrastructure.Services.FiscalDataValidator;

public interface IFiscalDataValidatorService
{
    Task<ErrorOr<FiscalDataEntry>> ValidateFiscalDataAsync(string taxId);
}
