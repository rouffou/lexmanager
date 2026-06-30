using Mediarq.UnitOfWork;

namespace LexManager.Modules.Identity.Application.Abstractions;

/// <summary>
/// Module-scoped unit of work (Mediarq's <see cref="IUnitOfWork"/>). A dedicated marker per module
/// keeps each DbContext's commit boundary distinct in DI — essential in a modular monolith where
/// several <c>IUnitOfWork</c> implementations coexist (one per module schema).
/// </summary>
public interface IIdentityUnitOfWork : IUnitOfWork;
