using Domain.Entities.Accounting;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Seed;

/// <summary>
/// Extension para aplicar seed data contable al ModelBuilder.
/// Se usa con owned types (AuditField) que requieren formato anónimo para HasData.
/// </summary>
public static class AccountingSeedExtension
{
    private static readonly DateTime SeedDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void SeedAccountingData(this ModelBuilder modelBuilder)
    {
        SeedCurrencies(modelBuilder);
        SeedExchangeRates(modelBuilder);
        SeedAccountCatalogs(modelBuilder);
        SeedAccountingPeriods(modelBuilder);
        SeedTaxConfigurations(modelBuilder);
    }

    private static void SeedCurrencies(ModelBuilder modelBuilder)
    {
        var gtqId = AccountingSeedData.CurrencyIds.GTQ;
        var usdId = AccountingSeedData.CurrencyIds.USD;

        modelBuilder.Entity<Currency>().HasData(
            new { Id = new CurrencyId(gtqId), Code = "GTQ", Name = "Quetzal Guatemalteco", Symbol = "Q", IsFunctional = true, DecimalPlaces = 2 },
            new { Id = new CurrencyId(usdId), Code = "USD", Name = "Dólar Estadounidense", Symbol = "$", IsFunctional = false, DecimalPlaces = 2 }
        );

        modelBuilder.Entity<Currency>().OwnsOne(c => c.AuditField).HasData(
            new { CurrencyId = new CurrencyId(gtqId), CreatedAt = SeedDate, UpdatedAt = (DateTime?)null, DeletedAt = (DateTime?)null, IsActive = true },
            new { CurrencyId = new CurrencyId(usdId), CreatedAt = SeedDate, UpdatedAt = (DateTime?)null, DeletedAt = (DateTime?)null, IsActive = true }
        );
    }

    private static void SeedExchangeRates(ModelBuilder modelBuilder)
    {
        var rateId = new ExchangeRateId(new Guid("c0a1b2c3-0002-4000-a000-000000000001"));

        modelBuilder.Entity<ExchangeRate>().HasData(
            new { Id = rateId, CurrencyId = new CurrencyId(AccountingSeedData.CurrencyIds.USD), Date = SeedDate, BuyRate = 7.66m, SellRate = 7.66m, Source = "Manual" }
        );

        modelBuilder.Entity<ExchangeRate>().OwnsOne(e => e.AuditField).HasData(
            new { ExchangeRateId = rateId, CreatedAt = SeedDate, UpdatedAt = (DateTime?)null, DeletedAt = (DateTime?)null, IsActive = true }
        );
    }

    private static void SeedAccountCatalogs(ModelBuilder modelBuilder)
    {
        var accounts = AccountingSeedData.GetAccountCatalogs();

        foreach (var account in accounts)
        {
            modelBuilder.Entity<AccountCatalog>().HasData(new
            {
                Id = account.Id,
                AccountCode = account.AccountCode,
                Name = account.Name,
                AccountType = account.AccountType,
                Nature = account.Nature,
                ParentId = account.ParentId,
                Level = account.Level,
                AcceptsMovements = account.AcceptsMovements
            });

            modelBuilder.Entity<AccountCatalog>().OwnsOne(a => a.AuditField).HasData(new
            {
                AccountCatalogId = account.Id,
                CreatedAt = SeedDate,
                UpdatedAt = (DateTime?)null,
                DeletedAt = (DateTime?)null,
                IsActive = true
            });
        }
    }

    private static void SeedAccountingPeriods(ModelBuilder modelBuilder)
    {
        var periodId = new AccountingPeriodId(new Guid("c0a1b2c3-0004-4000-a000-000000000001"));

        modelBuilder.Entity<AccountingPeriod>().HasData(new
        {
            Id = periodId,
            Name = "Año 2026",
            StartDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDate = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            Status = Domain.Entities.Accounting.Enums.PeriodStatus.Abierto
        });

        modelBuilder.Entity<AccountingPeriod>().OwnsOne(p => p.AuditField).HasData(new
        {
            AccountingPeriodId = periodId,
            CreatedAt = SeedDate,
            UpdatedAt = (DateTime?)null,
            DeletedAt = (DateTime?)null,
            IsActive = true
        });
    }

    private static void SeedTaxConfigurations(ModelBuilder modelBuilder)
    {
        var ivaId = new TaxConfigurationId(new Guid("c0a1b2c3-0003-4000-c000-000000000001"));
        var isrId = new TaxConfigurationId(new Guid("c0a1b2c3-0003-4000-c000-000000000002"));

        modelBuilder.Entity<TaxConfiguration>().HasData(
            new
            {
                Id = ivaId,
                Name = "IVA 12%",
                Percentage = 12.00m,
                TaxType = Domain.Entities.Accounting.Enums.TaxType.SobreVenta,
                DebitAccountId = new AccountCatalogId(AccountingSeedData.AccountIds.IvaPorCobrar),
                CreditAccountId = new AccountCatalogId(AccountingSeedData.AccountIds.IvaPorPagar)
            },
            new
            {
                Id = isrId,
                Name = "ISR Retención 5%",
                Percentage = 5.00m,
                TaxType = Domain.Entities.Accounting.Enums.TaxType.Retencion,
                DebitAccountId = new AccountCatalogId(AccountingSeedData.AccountIds.IsrPorPagar),
                CreditAccountId = new AccountCatalogId(AccountingSeedData.AccountIds.RetencionesIsr)
            }
        );

        modelBuilder.Entity<TaxConfiguration>().OwnsOne(t => t.AuditField).HasData(
            new { TaxConfigurationId = ivaId, CreatedAt = SeedDate, UpdatedAt = (DateTime?)null, DeletedAt = (DateTime?)null, IsActive = true },
            new { TaxConfigurationId = isrId, CreatedAt = SeedDate, UpdatedAt = (DateTime?)null, DeletedAt = (DateTime?)null, IsActive = true }
        );
    }
}
