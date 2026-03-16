using Domain.Primitives.Mediator;
using ErrorOr;

namespace Application.Features.Invoices.UploadContingency;

public record UploadContingencyCommand : IRequest<ErrorOr<UploadContingencyResult>>;

public record UploadContingencyResult(int Uploaded, int Failed, List<string> Details);
