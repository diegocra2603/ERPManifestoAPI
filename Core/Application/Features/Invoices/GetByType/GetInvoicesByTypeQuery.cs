using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.GetByType;

public record GetInvoicesByTypeQuery(int InvoiceType) : IRequest<ErrorOr<IReadOnlyList<InvoiceDto>>>;
