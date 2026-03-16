using Domain.Entities.Accounting.Enums;
using Domain.ValueObjects;

namespace Domain.Entities.Accounting;

/// <summary>
/// Seed data para el módulo contable con catálogo base guatemalteco.
/// </summary>
public static class AccountingSeedData
{
    private static readonly DateTime SeedCreatedAt = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly AuditField SeedAudit = new(SeedCreatedAt, null, null, true);

    // ==================== MONEDAS ====================
    public static class CurrencyIds
    {
        public static readonly Guid GTQ = new("c0a1b2c3-0001-4000-a000-000000000001");
        public static readonly Guid USD = new("c0a1b2c3-0001-4000-a000-000000000002");
    }

    public static IReadOnlyList<Currency> GetCurrencies() =>
    [
        new(new CurrencyId(CurrencyIds.GTQ), "GTQ", "Quetzal Guatemalteco", "Q", true, 2, SeedAudit),
        new(new CurrencyId(CurrencyIds.USD), "USD", "Dólar Estadounidense", "$", false, 2, SeedAudit),
    ];

    // ==================== TIPO DE CAMBIO ====================
    public static IReadOnlyList<ExchangeRate> GetExchangeRates() =>
    [
        new(new ExchangeRateId(new Guid("c0a1b2c3-0002-4000-a000-000000000001")),
            new CurrencyId(CurrencyIds.USD),
            new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            7.66m, 7.66m, "Manual", SeedAudit),
    ];

    // ==================== CATÁLOGO DE CUENTAS BASE GUATEMALA ====================
    public static class AccountIds
    {
        // Nivel 1 - Grupos principales
        public static readonly Guid Activos = new("a0a1b2c3-0001-4000-b000-000000000001");
        public static readonly Guid Pasivos = new("a0a1b2c3-0001-4000-b000-000000000002");
        public static readonly Guid Capital = new("a0a1b2c3-0001-4000-b000-000000000003");
        public static readonly Guid Ingresos = new("a0a1b2c3-0001-4000-b000-000000000004");
        public static readonly Guid Gastos = new("a0a1b2c3-0001-4000-b000-000000000005");
        public static readonly Guid Costos = new("a0a1b2c3-0001-4000-b000-000000000006");

        // Nivel 2 - Activos
        public static readonly Guid ActivoCorriente = new("a0a1b2c3-0002-4000-b000-000000000001");
        public static readonly Guid ActivoNoCorriente = new("a0a1b2c3-0002-4000-b000-000000000002");

        // Nivel 2 - Pasivos
        public static readonly Guid PasivoCorriente = new("a0a1b2c3-0002-4000-b000-000000000003");
        public static readonly Guid PasivoNoCorriente = new("a0a1b2c3-0002-4000-b000-000000000004");

        // Nivel 2 - Capital
        public static readonly Guid CapitalSocial = new("a0a1b2c3-0002-4000-b000-000000000005");
        public static readonly Guid ResultadosAcumulados = new("a0a1b2c3-0002-4000-b000-000000000006");

        // Nivel 2 - Ingresos
        public static readonly Guid IngresosOperativos = new("a0a1b2c3-0002-4000-b000-000000000007");
        public static readonly Guid IngresosNoOperativos = new("a0a1b2c3-0002-4000-b000-000000000008");

        // Nivel 2 - Gastos
        public static readonly Guid GastosOperativos = new("a0a1b2c3-0002-4000-b000-000000000009");
        public static readonly Guid GastosAdministrativos = new("a0a1b2c3-0002-4000-b000-000000000010");
        public static readonly Guid GastosFinancieros = new("a0a1b2c3-0002-4000-b000-000000000011");

        // Nivel 2 - Costos
        public static readonly Guid CostoDeVentas = new("a0a1b2c3-0002-4000-b000-000000000012");

        // Nivel 3 - Cuentas de movimiento (Activo Corriente)
        public static readonly Guid CajaGeneral = new("a0a1b2c3-0003-4000-b000-000000000001");
        public static readonly Guid BancosNacional = new("a0a1b2c3-0003-4000-b000-000000000002");
        public static readonly Guid CuentasPorCobrar = new("a0a1b2c3-0003-4000-b000-000000000003");
        public static readonly Guid IvaPorCobrar = new("a0a1b2c3-0003-4000-b000-000000000004");
        public static readonly Guid Inventarios = new("a0a1b2c3-0003-4000-b000-000000000005");
        public static readonly Guid AnticiposAProveedores = new("a0a1b2c3-0003-4000-b000-000000000006");

        // Nivel 3 - Cuentas de movimiento (Activo No Corriente)
        public static readonly Guid MobiliarioYEquipo = new("a0a1b2c3-0003-4000-b000-000000000007");
        public static readonly Guid EquipoDeComputo = new("a0a1b2c3-0003-4000-b000-000000000008");
        public static readonly Guid Vehiculos = new("a0a1b2c3-0003-4000-b000-000000000009");
        public static readonly Guid DepreciacionAcumulada = new("a0a1b2c3-0003-4000-b000-000000000010");

        // Nivel 3 - Cuentas de movimiento (Pasivo Corriente)
        public static readonly Guid CuentasPorPagar = new("a0a1b2c3-0003-4000-b000-000000000011");
        public static readonly Guid IvaPorPagar = new("a0a1b2c3-0003-4000-b000-000000000012");
        public static readonly Guid IsrPorPagar = new("a0a1b2c3-0003-4000-b000-000000000013");
        public static readonly Guid SueldosPorPagar = new("a0a1b2c3-0003-4000-b000-000000000014");
        public static readonly Guid RetencionesIsr = new("a0a1b2c3-0003-4000-b000-000000000015");

        // Nivel 3 - Pasivo No Corriente
        public static readonly Guid PrestamosBancarios = new("a0a1b2c3-0003-4000-b000-000000000016");

        // Nivel 3 - Capital
        public static readonly Guid CapitalAutorizado = new("a0a1b2c3-0003-4000-b000-000000000017");
        public static readonly Guid ReservaLegal = new("a0a1b2c3-0003-4000-b000-000000000018");
        public static readonly Guid UtilidadDelEjercicio = new("a0a1b2c3-0003-4000-b000-000000000019");
        public static readonly Guid UtilidadesRetenidas = new("a0a1b2c3-0003-4000-b000-000000000020");

        // Nivel 3 - Ingresos
        public static readonly Guid VentasDeBienes = new("a0a1b2c3-0003-4000-b000-000000000021");
        public static readonly Guid VentasDeServicios = new("a0a1b2c3-0003-4000-b000-000000000022");
        public static readonly Guid OtrosIngresos = new("a0a1b2c3-0003-4000-b000-000000000023");
        public static readonly Guid IngresosFinancieros = new("a0a1b2c3-0003-4000-b000-000000000024");

        // Nivel 3 - Gastos
        public static readonly Guid GastosSueldos = new("a0a1b2c3-0003-4000-b000-000000000025");
        public static readonly Guid GastosAlquiler = new("a0a1b2c3-0003-4000-b000-000000000026");
        public static readonly Guid GastosServicios = new("a0a1b2c3-0003-4000-b000-000000000027");
        public static readonly Guid GastosDepreciacion = new("a0a1b2c3-0003-4000-b000-000000000028");
        public static readonly Guid GastosAdminSueldos = new("a0a1b2c3-0003-4000-b000-000000000029");
        public static readonly Guid GastosAdminAlquiler = new("a0a1b2c3-0003-4000-b000-000000000030");
        public static readonly Guid GastosAdminServicios = new("a0a1b2c3-0003-4000-b000-000000000031");
        public static readonly Guid InteresesBancarios = new("a0a1b2c3-0003-4000-b000-000000000032");
        public static readonly Guid ComisionesBancarias = new("a0a1b2c3-0003-4000-b000-000000000033");
        public static readonly Guid DiferencialCambiario = new("a0a1b2c3-0003-4000-b000-000000000034");

        // Nivel 3 - Costos
        public static readonly Guid ComprasDeMercancias = new("a0a1b2c3-0003-4000-b000-000000000035");
        public static readonly Guid CostoDeMateriaPrima = new("a0a1b2c3-0003-4000-b000-000000000036");
    }

    public static IReadOnlyList<AccountCatalog> GetAccountCatalogs()
    {
        var accounts = new List<AccountCatalog>();

        // Nivel 1 - Grupos principales
        accounts.Add(CreateAccount(AccountIds.Activos, "1", "Activos", AccountType.Activo, AccountNature.Deudora, null, 1, false));
        accounts.Add(CreateAccount(AccountIds.Pasivos, "2", "Pasivos", AccountType.Pasivo, AccountNature.Acreedora, null, 1, false));
        accounts.Add(CreateAccount(AccountIds.Capital, "3", "Capital", AccountType.Capital, AccountNature.Acreedora, null, 1, false));
        accounts.Add(CreateAccount(AccountIds.Ingresos, "4", "Ingresos", AccountType.Ingreso, AccountNature.Acreedora, null, 1, false));
        accounts.Add(CreateAccount(AccountIds.Gastos, "5", "Gastos", AccountType.Gasto, AccountNature.Deudora, null, 1, false));
        accounts.Add(CreateAccount(AccountIds.Costos, "6", "Costos", AccountType.Costo, AccountNature.Deudora, null, 1, false));

        // Nivel 2 - Sub-grupos
        accounts.Add(CreateAccount(AccountIds.ActivoCorriente, "1.1", "Activo Corriente", AccountType.Activo, AccountNature.Deudora, AccountIds.Activos, 2, false));
        accounts.Add(CreateAccount(AccountIds.ActivoNoCorriente, "1.2", "Activo No Corriente", AccountType.Activo, AccountNature.Deudora, AccountIds.Activos, 2, false));
        accounts.Add(CreateAccount(AccountIds.PasivoCorriente, "2.1", "Pasivo Corriente", AccountType.Pasivo, AccountNature.Acreedora, AccountIds.Pasivos, 2, false));
        accounts.Add(CreateAccount(AccountIds.PasivoNoCorriente, "2.2", "Pasivo No Corriente", AccountType.Pasivo, AccountNature.Acreedora, AccountIds.Pasivos, 2, false));
        accounts.Add(CreateAccount(AccountIds.CapitalSocial, "3.1", "Capital Social", AccountType.Capital, AccountNature.Acreedora, AccountIds.Capital, 2, false));
        accounts.Add(CreateAccount(AccountIds.ResultadosAcumulados, "3.2", "Resultados Acumulados", AccountType.Capital, AccountNature.Acreedora, AccountIds.Capital, 2, false));
        accounts.Add(CreateAccount(AccountIds.IngresosOperativos, "4.1", "Ingresos Operativos", AccountType.Ingreso, AccountNature.Acreedora, AccountIds.Ingresos, 2, false));
        accounts.Add(CreateAccount(AccountIds.IngresosNoOperativos, "4.2", "Ingresos No Operativos", AccountType.Ingreso, AccountNature.Acreedora, AccountIds.Ingresos, 2, false));
        accounts.Add(CreateAccount(AccountIds.GastosOperativos, "5.1", "Gastos de Operación", AccountType.Gasto, AccountNature.Deudora, AccountIds.Gastos, 2, false));
        accounts.Add(CreateAccount(AccountIds.GastosAdministrativos, "5.2", "Gastos de Administración", AccountType.Gasto, AccountNature.Deudora, AccountIds.Gastos, 2, false));
        accounts.Add(CreateAccount(AccountIds.GastosFinancieros, "5.3", "Gastos Financieros", AccountType.Gasto, AccountNature.Deudora, AccountIds.Gastos, 2, false));
        accounts.Add(CreateAccount(AccountIds.CostoDeVentas, "6.1", "Costo de Ventas", AccountType.Costo, AccountNature.Deudora, AccountIds.Costos, 2, false));

        // Nivel 3 - Cuentas de movimiento (Activo Corriente)
        accounts.Add(CreateAccount(AccountIds.CajaGeneral, "1.1.01", "Caja General", AccountType.Activo, AccountNature.Deudora, AccountIds.ActivoCorriente, 3, true));
        accounts.Add(CreateAccount(AccountIds.BancosNacional, "1.1.02", "Bancos Nacional", AccountType.Activo, AccountNature.Deudora, AccountIds.ActivoCorriente, 3, true));
        accounts.Add(CreateAccount(AccountIds.CuentasPorCobrar, "1.1.03", "Cuentas por Cobrar", AccountType.Activo, AccountNature.Deudora, AccountIds.ActivoCorriente, 3, true));
        accounts.Add(CreateAccount(AccountIds.IvaPorCobrar, "1.1.04", "IVA por Cobrar (Crédito Fiscal)", AccountType.Activo, AccountNature.Deudora, AccountIds.ActivoCorriente, 3, true));
        accounts.Add(CreateAccount(AccountIds.Inventarios, "1.1.05", "Inventarios", AccountType.Activo, AccountNature.Deudora, AccountIds.ActivoCorriente, 3, true));
        accounts.Add(CreateAccount(AccountIds.AnticiposAProveedores, "1.1.06", "Anticipos a Proveedores", AccountType.Activo, AccountNature.Deudora, AccountIds.ActivoCorriente, 3, true));

        // Nivel 3 - Activo No Corriente
        accounts.Add(CreateAccount(AccountIds.MobiliarioYEquipo, "1.2.01", "Mobiliario y Equipo", AccountType.Activo, AccountNature.Deudora, AccountIds.ActivoNoCorriente, 3, true));
        accounts.Add(CreateAccount(AccountIds.EquipoDeComputo, "1.2.02", "Equipo de Cómputo", AccountType.Activo, AccountNature.Deudora, AccountIds.ActivoNoCorriente, 3, true));
        accounts.Add(CreateAccount(AccountIds.Vehiculos, "1.2.03", "Vehículos", AccountType.Activo, AccountNature.Deudora, AccountIds.ActivoNoCorriente, 3, true));
        accounts.Add(CreateAccount(AccountIds.DepreciacionAcumulada, "1.2.04", "Depreciación Acumulada", AccountType.Activo, AccountNature.Acreedora, AccountIds.ActivoNoCorriente, 3, true));

        // Nivel 3 - Pasivo Corriente
        accounts.Add(CreateAccount(AccountIds.CuentasPorPagar, "2.1.01", "Cuentas por Pagar", AccountType.Pasivo, AccountNature.Acreedora, AccountIds.PasivoCorriente, 3, true));
        accounts.Add(CreateAccount(AccountIds.IvaPorPagar, "2.1.02", "IVA por Pagar (Débito Fiscal)", AccountType.Pasivo, AccountNature.Acreedora, AccountIds.PasivoCorriente, 3, true));
        accounts.Add(CreateAccount(AccountIds.IsrPorPagar, "2.1.03", "ISR por Pagar", AccountType.Pasivo, AccountNature.Acreedora, AccountIds.PasivoCorriente, 3, true));
        accounts.Add(CreateAccount(AccountIds.SueldosPorPagar, "2.1.04", "Sueldos por Pagar", AccountType.Pasivo, AccountNature.Acreedora, AccountIds.PasivoCorriente, 3, true));
        accounts.Add(CreateAccount(AccountIds.RetencionesIsr, "2.1.05", "Retenciones de ISR", AccountType.Pasivo, AccountNature.Acreedora, AccountIds.PasivoCorriente, 3, true));

        // Nivel 3 - Pasivo No Corriente
        accounts.Add(CreateAccount(AccountIds.PrestamosBancarios, "2.2.01", "Préstamos Bancarios", AccountType.Pasivo, AccountNature.Acreedora, AccountIds.PasivoNoCorriente, 3, true));

        // Nivel 3 - Capital
        accounts.Add(CreateAccount(AccountIds.CapitalAutorizado, "3.1.01", "Capital Autorizado", AccountType.Capital, AccountNature.Acreedora, AccountIds.CapitalSocial, 3, true));
        accounts.Add(CreateAccount(AccountIds.ReservaLegal, "3.1.02", "Reserva Legal", AccountType.Capital, AccountNature.Acreedora, AccountIds.CapitalSocial, 3, true));
        accounts.Add(CreateAccount(AccountIds.UtilidadDelEjercicio, "3.2.01", "Utilidad del Ejercicio", AccountType.Capital, AccountNature.Acreedora, AccountIds.ResultadosAcumulados, 3, true));
        accounts.Add(CreateAccount(AccountIds.UtilidadesRetenidas, "3.2.02", "Utilidades Retenidas", AccountType.Capital, AccountNature.Acreedora, AccountIds.ResultadosAcumulados, 3, true));

        // Nivel 3 - Ingresos
        accounts.Add(CreateAccount(AccountIds.VentasDeBienes, "4.1.01", "Ventas de Bienes", AccountType.Ingreso, AccountNature.Acreedora, AccountIds.IngresosOperativos, 3, true));
        accounts.Add(CreateAccount(AccountIds.VentasDeServicios, "4.1.02", "Ventas de Servicios", AccountType.Ingreso, AccountNature.Acreedora, AccountIds.IngresosOperativos, 3, true));
        accounts.Add(CreateAccount(AccountIds.OtrosIngresos, "4.2.01", "Otros Ingresos", AccountType.Ingreso, AccountNature.Acreedora, AccountIds.IngresosNoOperativos, 3, true));
        accounts.Add(CreateAccount(AccountIds.IngresosFinancieros, "4.2.02", "Ingresos Financieros", AccountType.Ingreso, AccountNature.Acreedora, AccountIds.IngresosNoOperativos, 3, true));

        // Nivel 3 - Gastos Operativos
        accounts.Add(CreateAccount(AccountIds.GastosSueldos, "5.1.01", "Sueldos y Salarios", AccountType.Gasto, AccountNature.Deudora, AccountIds.GastosOperativos, 3, true));
        accounts.Add(CreateAccount(AccountIds.GastosAlquiler, "5.1.02", "Alquiler de Local", AccountType.Gasto, AccountNature.Deudora, AccountIds.GastosOperativos, 3, true));
        accounts.Add(CreateAccount(AccountIds.GastosServicios, "5.1.03", "Servicios Básicos", AccountType.Gasto, AccountNature.Deudora, AccountIds.GastosOperativos, 3, true));
        accounts.Add(CreateAccount(AccountIds.GastosDepreciacion, "5.1.04", "Depreciaciones", AccountType.Gasto, AccountNature.Deudora, AccountIds.GastosOperativos, 3, true));

        // Nivel 3 - Gastos Administrativos
        accounts.Add(CreateAccount(AccountIds.GastosAdminSueldos, "5.2.01", "Sueldos Administrativos", AccountType.Gasto, AccountNature.Deudora, AccountIds.GastosAdministrativos, 3, true));
        accounts.Add(CreateAccount(AccountIds.GastosAdminAlquiler, "5.2.02", "Alquiler Oficina", AccountType.Gasto, AccountNature.Deudora, AccountIds.GastosAdministrativos, 3, true));
        accounts.Add(CreateAccount(AccountIds.GastosAdminServicios, "5.2.03", "Servicios Administrativos", AccountType.Gasto, AccountNature.Deudora, AccountIds.GastosAdministrativos, 3, true));

        // Nivel 3 - Gastos Financieros
        accounts.Add(CreateAccount(AccountIds.InteresesBancarios, "5.3.01", "Intereses Bancarios", AccountType.Gasto, AccountNature.Deudora, AccountIds.GastosFinancieros, 3, true));
        accounts.Add(CreateAccount(AccountIds.ComisionesBancarias, "5.3.02", "Comisiones Bancarias", AccountType.Gasto, AccountNature.Deudora, AccountIds.GastosFinancieros, 3, true));
        accounts.Add(CreateAccount(AccountIds.DiferencialCambiario, "5.3.03", "Diferencial Cambiario", AccountType.Gasto, AccountNature.Deudora, AccountIds.GastosFinancieros, 3, true));

        // Nivel 3 - Costos
        accounts.Add(CreateAccount(AccountIds.ComprasDeMercancias, "6.1.01", "Compras de Mercancías", AccountType.Costo, AccountNature.Deudora, AccountIds.CostoDeVentas, 3, true));
        accounts.Add(CreateAccount(AccountIds.CostoDeMateriaPrima, "6.1.02", "Costo de Materia Prima", AccountType.Costo, AccountNature.Deudora, AccountIds.CostoDeVentas, 3, true));

        return accounts.AsReadOnly();
    }

    // ==================== IMPUESTOS ====================
    public static IReadOnlyList<TaxConfiguration> GetTaxConfigurations() =>
    [
        new(new TaxConfigurationId(new Guid("c0a1b2c3-0003-4000-c000-000000000001")),
            "IVA 12%", 12.00m, TaxType.SobreVenta,
            new AccountCatalogId(AccountIds.IvaPorCobrar),
            new AccountCatalogId(AccountIds.IvaPorPagar),
            SeedAudit),
        new(new TaxConfigurationId(new Guid("c0a1b2c3-0003-4000-c000-000000000002")),
            "ISR Retención 5%", 5.00m, TaxType.Retencion,
            new AccountCatalogId(AccountIds.IsrPorPagar),
            new AccountCatalogId(AccountIds.RetencionesIsr),
            SeedAudit),
    ];

    // ==================== PERÍODO CONTABLE INICIAL ====================
    public static IReadOnlyList<AccountingPeriod> GetAccountingPeriods() =>
    [
        new(new AccountingPeriodId(new Guid("c0a1b2c3-0004-4000-a000-000000000001")),
            "Año 2026", new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc),
            PeriodStatus.Abierto, SeedAudit),
    ];

    private static AccountCatalog CreateAccount(Guid id, string code, string name,
        AccountType type, AccountNature nature, Guid? parentId, int level, bool acceptsMovements)
    {
        return new AccountCatalog(
            new AccountCatalogId(id),
            code, name, type, nature,
            parentId.HasValue ? new AccountCatalogId(parentId.Value) : null,
            level, acceptsMovements, SeedAudit);
    }
}
