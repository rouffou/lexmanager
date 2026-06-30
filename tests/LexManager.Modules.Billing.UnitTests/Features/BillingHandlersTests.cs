using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Application.Features.CreateBillingDocument;
using LexManager.Modules.Billing.Application.Features.GenerateTimeBasedInvoice;
using LexManager.Modules.Billing.Domain.Billing;
using LexManager.Modules.Billing.Domain.Common;
using LexManager.Modules.Calendar.Contracts;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.Identity.Contracts;
using NSubstitute;

namespace LexManager.Modules.Billing.UnitTests.Features;

public class BillingHandlersTests
{
    private readonly IBillingDocumentRepository _repository = Substitute.For<IBillingDocumentRepository>();
    private readonly ICaseApi _caseApi = Substitute.For<ICaseApi>();
    private readonly IClientApi _clientApi = Substitute.For<IClientApi>();
    private readonly ITimeTrackingApi _timeApi = Substitute.For<ITimeTrackingApi>();
    private readonly IInvoiceNumberGenerator _numbers = Substitute.For<IInvoiceNumberGenerator>();
    private readonly IBillingUnitOfWork _unitOfWork = Substitute.For<IBillingUnitOfWork>();

    [Fact]
    public async Task CreateBillingDocument_Should_Fail_WhenCaseMissing()
    {
        _caseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);
        var handler = new CreateBillingDocumentCommandHandler(_repository, _caseApi, _clientApi, _unitOfWork);

        var result = await handler.Handle(
            new CreateBillingDocumentCommand(Guid.NewGuid(), Guid.NewGuid(), BillingDocumentKind.Invoice, BillingMode.Flat),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BillingErrors.CaseNotFound);
    }

    [Fact]
    public async Task CreateBillingDocument_Should_Fail_WhenClientMissing()
    {
        _caseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _clientApi.ClientExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);
        var handler = new CreateBillingDocumentCommandHandler(_repository, _caseApi, _clientApi, _unitOfWork);

        var result = await handler.Handle(
            new CreateBillingDocumentCommand(Guid.NewGuid(), Guid.NewGuid(), BillingDocumentKind.Invoice, BillingMode.Flat),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BillingErrors.ClientNotFound);
    }

    [Fact]
    public async Task GenerateTimeBasedInvoice_Should_Fail_WhenNoBillableTime()
    {
        _caseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _clientApi.ClientExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _timeApi.GetCaseTimeSummaryAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new CaseTimeSummaryResponse(Guid.NewGuid(), 0, 0, 0));

        var handler = new GenerateTimeBasedInvoiceCommandHandler(_repository, _caseApi, _clientApi, _timeApi, _numbers, _unitOfWork);
        var result = await handler.Handle(
            new GenerateTimeBasedInvoiceCommand(Guid.NewGuid(), Guid.NewGuid(), 200m, DateTime.UtcNow.AddDays(30)),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BillingErrors.NoBillableTime);
    }

    [Fact]
    public async Task GenerateTimeBasedInvoice_Should_IssueDocument_FromBillableTime()
    {
        _caseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _clientApi.ClientExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _timeApi.GetCaseTimeSummaryAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new CaseTimeSummaryResponse(Guid.NewGuid(), 120, 90, 2)); // 90 billable min = 1.5h
        _numbers.NextAsync(Arg.Any<BillingDocumentKind>(), Arg.Any<CancellationToken>()).Returns("NOTE-2026-000001");

        BillingDocument? added = null;
        _repository.When(r => r.Add(Arg.Any<BillingDocument>())).Do(call => added = call.Arg<BillingDocument>());

        var handler = new GenerateTimeBasedInvoiceCommandHandler(_repository, _caseApi, _clientApi, _timeApi, _numbers, _unitOfWork);
        var result = await handler.Handle(
            new GenerateTimeBasedInvoiceCommand(Guid.NewGuid(), Guid.NewGuid(), 200m, DateTime.UtcNow.AddDays(30)),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        added.Should().NotBeNull();
        added!.Status.Should().Be(BillingStatus.Issued);
        added.Mode.Should().Be(BillingMode.TimeBased);
        added.Subtotal.Amount.Should().Be(300m); // 1.5h * 200
        added.Total.Amount.Should().Be(360m);     // + 20% VAT
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
