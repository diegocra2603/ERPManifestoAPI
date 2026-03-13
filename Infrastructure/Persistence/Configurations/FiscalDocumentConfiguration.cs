using Domain.Entities.FiscalDocuments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public sealed class FiscalDocumentEntityConfiguration : IEntityTypeConfiguration<FiscalDocument>
{
    public void Configure(EntityTypeBuilder<FiscalDocument> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasConversion(id => id.Value, v => new FiscalDocumentId(v));

        builder.Property(f => f.DocumentType)
            .IsRequired();

        builder.Property(f => f.Status)
            .IsRequired();

        builder.Property(f => f.NitReceptor)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(f => f.NombreReceptor)
            .HasMaxLength(200);

        builder.Property(f => f.DireccionReceptor)
            .HasMaxLength(200);

        builder.Property(f => f.TipoVenta)
            .IsRequired();

        builder.Property(f => f.DestinoVenta)
            .IsRequired();

        builder.Property(f => f.Fecha)
            .IsRequired();

        builder.Property(f => f.Moneda)
            .IsRequired();

        builder.Property(f => f.Tasa)
            .HasPrecision(10, 6)
            .IsRequired();

        builder.Property(f => f.Referencia)
            .HasMaxLength(40)
            .IsRequired();

        builder.HasIndex(f => f.Referencia);

        builder.Property(f => f.NumeroAcceso);

        builder.Property(f => f.SerieAdmin)
            .HasMaxLength(20);

        builder.Property(f => f.NumeroAdmin);

        builder.Property(f => f.Bruto)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(f => f.Descuento)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(f => f.Exento)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(f => f.Otros)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(f => f.Neto)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(f => f.Isr)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(f => f.Iva)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(f => f.Total)
            .HasPrecision(20, 2)
            .IsRequired();

        builder.Property(f => f.Serie)
            .HasMaxLength(50);

        builder.Property(f => f.Preimpreso)
            .HasMaxLength(50);

        builder.Property(f => f.NumeroAutorizacion)
            .HasMaxLength(100);

        builder.HasIndex(f => f.NumeroAutorizacion);

        builder.Property(f => f.DocAsociadoSerie)
            .HasMaxLength(50);

        builder.Property(f => f.DocAsociadoPreimpreso)
            .HasMaxLength(50);

        builder.Property(f => f.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(f => f.XmlEnviado);

        builder.HasMany(f => f.Items)
            .WithOne(i => i.FiscalDocument)
            .HasForeignKey(i => i.FiscalDocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsOne(t => t.AuditField, audit =>
        {
            audit.Property(a => a.CreatedAt)
                .IsRequired();

            audit.Property(a => a.UpdatedAt);

            audit.Property(a => a.DeletedAt);

            audit.Property(a => a.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        });
    }
}
