using LexManager.Modules.Billing.Application.Abstractions;
using LexManager.Modules.Billing.Application.Features.OpenCarpaAccount;
using LexManager.Modules.Billing.Application.Features.RecordCarpaDeposit;
using LexManager.Modules.Billing.Application.Features.RecordCarpaDisbursement;
using LexManager.Modules.Billing.Domain.Carpa;
using LexManager.Modules.Billing.Domain.Common;
using LexManager.Modules.CaseManagement.Contracts;
using LexManager.Modules.Identity.Contracts;
using NSubstitute;

namespace LexManager.Modules.Billing.UnitTests.Features;

public class CarpaHandlersTests
{
    private readonly ICarpaAccountRepository _repository = Substitute.For<ICarpaAccountRepository>();
    private readonly ICaseApi _caseApi = Substitute.For<ICaseApi>();
    private readonly IClientApi _clientApi = Substitute.For<IClientApi>();
    private readonly IBillingUnitOfWork _unitOfWork = Substitute.For<IBillingUnitOfWork>();

    [Fact]
    public async Task OpenCarpaAccount_Should_Fail_WhenCaseMissing()
    {
        _caseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);
        var handler = new OpenCarpaAccountCommandHandler(_repository, _caseApi, _clientApi, _unitOfWork);

        var result = await handler.Handle(new OpenCarpaAccountCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CarpaErrors.CaseNotFound);
    }

    [Fact]
    public async Task OpenCarpaAccount_Should_Fail_WhenClientMissing()
    {
        _caseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _clientApi.ClientExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);
        var handler = new OpenCarpaAccountCommandHandler(_repository, _caseApi, _clientApi, _unitOfWork);

        var result = await handler.Handle(new OpenCarpaAccountCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CarpaErrors.ClientNotFound);
    }

    [Fact]
    public async Task OpenCarpaAccount_Should_Fail_WhenAccountAlreadyExists()
    {
        _caseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _clientApi.ClientExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _repository.ExistsForCaseAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        var handler = new OpenCarpaAccountCommandHandler(_repository, _caseApi, _clientApi, _unitOfWork);

        var result = await handler.Handle(new OpenCarpaAccountCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CarpaErrors.AlreadyExistsForCase);
    }

    [Fact]
    public async Task OpenCarpaAccount_Should_Persist_WhenValid()
    {
        _caseApi.CaseExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _clientApi.ClientExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _repository.ExistsForCaseAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        CarpaAccount? added = null;
        _repository.When(r => r.Add(Arg.Any<CarpaAccount>())).Do(call => added = call.Arg<CarpaAccount>());

        var handler = new OpenCarpaAccountCommandHandler(_repository, _caseApi, _clientApi, _unitOfWork);
        var result = await handler.Handle(new OpenCarpaAccountCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        added.Should().NotBeNull();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RecordCarpaDeposit_Should_Fail_WhenAccountMissing()
    {
        _repository.GetByIdAsync(Arg.Any<CarpaAccountId>(), Arg.Any<CancellationToken>()).Returns((CarpaAccount?)null);
        var handler = new RecordCarpaDepositCommandHandler(_repository, _unitOfWork);

        var result = await handler.Handle(
            new RecordCarpaDepositCommand(Guid.NewGuid(), 100m, "Provision", null), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CarpaErrors.NotFound);
    }

    [Fact]
    public async Task RecordCarpaDeposit_Should_Persist_WhenValid()
    {
        CarpaAccount account = CarpaAccount.Open(Guid.NewGuid(), Guid.NewGuid());
        _repository.GetByIdAsync(Arg.Any<CarpaAccountId>(), Arg.Any<CancellationToken>()).Returns(account);
        var handler = new RecordCarpaDepositCommandHandler(_repository, _unitOfWork);

        var result = await handler.Handle(
            new RecordCarpaDepositCommand(account.Id.Value, 250m, "Provision", "Client"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        account.Balance.Amount.Should().Be(250m);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RecordCarpaDisbursement_Should_Fail_WhenAccountMissing()
    {
        _repository.GetByIdAsync(Arg.Any<CarpaAccountId>(), Arg.Any<CancellationToken>()).Returns((CarpaAccount?)null);
        var handler = new RecordCarpaDisbursementCommandHandler(_repository, _unitOfWork);

        var result = await handler.Handle(
            new RecordCarpaDisbursementCommand(Guid.NewGuid(), 100m, "Frais", null), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CarpaErrors.NotFound);
    }

    [Fact]
    public async Task RecordCarpaDisbursement_Should_Fail_WhenInsufficientFunds()
    {
        CarpaAccount account = CarpaAccount.Open(Guid.NewGuid(), Guid.NewGuid());
        account.Deposit(Money.Of(50m), "Petite provision");
        _repository.GetByIdAsync(Arg.Any<CarpaAccountId>(), Arg.Any<CancellationToken>()).Returns(account);
        var handler = new RecordCarpaDisbursementCommandHandler(_repository, _unitOfWork);

        var result = await handler.Handle(
            new RecordCarpaDisbursementCommand(account.Id.Value, 100m, "Frais", null), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CarpaErrors.InsufficientFunds);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RecordCarpaDisbursement_Should_Persist_WhenWithinBalance()
    {
        CarpaAccount account = CarpaAccount.Open(Guid.NewGuid(), Guid.NewGuid());
        account.Deposit(Money.Of(500m), "Provision");
        _repository.GetByIdAsync(Arg.Any<CarpaAccountId>(), Arg.Any<CancellationToken>()).Returns(account);
        var handler = new RecordCarpaDisbursementCommandHandler(_repository, _unitOfWork);

        var result = await handler.Handle(
            new RecordCarpaDisbursementCommand(account.Id.Value, 200m, "Versement", "Tiers"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        account.Balance.Amount.Should().Be(300m);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
