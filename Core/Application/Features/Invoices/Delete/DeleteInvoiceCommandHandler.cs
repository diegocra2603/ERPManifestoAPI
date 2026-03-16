using Domain.Contracts.Infrastructure.Persistence;
using Domain.Entities.Accounting;
using Domain.Entities.Accounting.Enums;
using Domain.Primitives;
using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.Delete;

public class DeleteInvoiceCommandHandler : IRequestHandler<DeleteInvoiceCommand, ErrorOr<bool>>
{
    private readonly IAsyncRepository<Invoice> _invoiceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteInvoiceCommandHandler(
        IAsyncRepository<Invoice> invoiceRepository,
        IUnitOfWork unitOfWork)
    {
        _invoiceRepository = invoiceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<bool>> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.FirstOrDefaultAsync(
            i => i.Id == new InvoiceId(request.Id) && i.AuditField.IsActive);

        if (invoice is null)
            return Error.NotFound("Invoice.NotFound", "Factura no encontrada.");

        if (invoice.Status != InvoiceStatus.Borrador)
            return Error.Validation("Invoice.CannotDelete", "Solo se pueden eliminar facturas en estado Borrador.");

        invoice.MarkAsDeleted();
        _invoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
