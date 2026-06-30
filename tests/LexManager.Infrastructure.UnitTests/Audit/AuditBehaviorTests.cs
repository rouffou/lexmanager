using LexManager.Infrastructure.Audit;
using LexManager.Infrastructure.Security;
using Mediarq.Core.Common.Results;
using Mediarq.Core.Mediators;
using Mediarq.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace LexManager.Infrastructure.UnitTests.Audit;

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
