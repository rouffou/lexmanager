using LexManager.Modules.Identity.Domain.Clients;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;

namespace LexManager.Modules.Identity.Application.Features.CreateClient;

/// <summary>
/// Creates a client — a natural person or a legal entity. The mandatory conflict-of-interest
/// search runs in the handler before persistence (SRD Module 1).
/// </summary>
public sealed record CreateClientCommand(
    ClientType Type,
    string Email,
    string? Phone,
    // Natural person
    string? FirstName,
    string? LastName,
    string? NationalIdentityNumber,
    // Legal entity
    string? CompanyName,
    string? RegistrationNumber,
    string? LegalRepresentative) : ICommand<Result<Guid>>;
