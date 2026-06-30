using LexManager.Infrastructure.Audit;
using LexManager.Infrastructure.Security;
using Mediarq.Core.Common.Requests.Command;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace LexManager.Infrastructure.UnitTests.Audit;

public sealed record PingCommand(string Message) : ICommand<Result<string>>;

public sealed class PingHandler : ICommandHandler<PingCommand, Result<string>>
{
    public Task<Result<string>> Handle(PingCommand request, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Success(request.Message));
}

public sealed class RecordingAuditSink : IAuditSink
{
    public List<AuditEntry> Entries { get; } = [];

    public Task WriteAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        Entries.Add(entry);
        return Task.CompletedTask;
    }
}

public class AuditBehaviorTests
{
    [Fact]
    public async Task Audit_Should_RecordCommand_AsItFlowsThroughThePipeline()
    {
        var sink = new RecordingAuditSink();
        var currentUser = Substitute.For<ICurrentUser>();
        var userId = Guid.NewGuid();
        currentUser.UserId.Returns(userId);

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<ICurrentUser>(currentUser);
        services.AddSingleton<IAuditSink>(sink);
        services.AddMediarq(isHttp: false, typeof(AuditBehaviorTests).Assembly);
        services.AddLexManagerAudit();

        await using ServiceProvider provider = services.BuildServiceProvider();
        ISender sender = provider.GetRequiredService<ISender>();

        Result<string> result = await sender.Send(new PingCommand("hello"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        sink.Entries.Should().ContainSingle();
        AuditEntry entry = sink.Entries.Single();
        entry.Action.Should().Be(nameof(PingCommand));
        entry.Outcome.Should().Be("Success");
        entry.UserId.Should().Be(userId);
    }
}
