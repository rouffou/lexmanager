namespace LexManager.Application.Abstractions.Persistence;

/// <summary>
/// Commits the changes tracked within a single business transaction. Each module's DbContext
/// implements this; command handlers depend on the abstraction, not on EF Core.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
