using AwesomeAssertions;
using MiniBank.Accounts.Domain.Accounts;
using MiniBank.Accounts.Domain.Accounts.Events;
using MiniBank.Accounts.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniBank.Accounts.Domain.Tests;

public class AccountTests
{
    private static readonly CustomerId customerId = new("customer-1");

    private static Money Gel(decimal amount) => new(amount, Currency.GEL);
    private static Money Usd(decimal amount) => new(amount, Currency.USD);

    private static Account OpenGelAccount() => Account.Open(AccountId.New(), customerId, Currency.GEL);

    // ---------- Opening ----------

    [Fact]
    public void Open_WithValidOwner_StartsOpenWithZeroBalance()
    {
        var id = AccountId.New();

        var account = Account.Open(id, customerId, Currency.GEL);

        account.Id.Should().Be(id);
        account.OwnerId.Should().Be(customerId);
        account.Currency.Should().Be(Currency.GEL);
        account.Balance.Should().Be(Gel(0));
        account.Status.Should().Be(AccountStatus.Open);
    }

    [Fact]
    public void Open_RaisesAccountOpenedEvent_AsVersionZero()
    {
        var account = OpenGelAccount();

        account.Version.Should().Be(0);
        account.UncommittedEvents.Should().ContainSingle()
            .Which.Should().BeOfType<AccountOpened>();
    }

    [Fact]
    public void Open_WithEmptyOwner_Throws()
    {
        var act = () => Account.Open(AccountId.New(), new CustomerId(""), Currency.GEL);

        act.Should().Throw<DomainException>().WithMessage("*owner*");
    }

    // ---------- Deposit ----------

    [Fact]
    public void Deposit_PositiveAmount_IncreaseBalance()
    {
        var account = OpenGelAccount();

        account.Deposit(Gel(100));

        account.Balance.Should().Be(Gel(100));
        account.UncommittedEvents.Last().Should().BeOfType<MoneyDeposited>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Deposit_ZeroOrNegativeAmount_Throws_AndChangesNothing(int amount)
    {
        var account = OpenGelAccount();

        var act = () => account.Deposit(Gel(amount));

        act.Should().Throw<DomainException>().WithMessage("*positive*");
        account.Balance.Should().Be(Gel(0));
        account.UncommittedEvents.Should().HaveCount(1, "only AccountOpened should exist");
    }

    // ---------- Withdraw ----------

    [Fact]
    public void Withdraw_LessThanBalance_DecreasesBalance()
    {
        var account = OpenGelAccount();
        account.Deposit(Gel(100));

        account.Withdraw(Gel(30));

        account.Balance.Should().Be(Gel(70));
    }

    [Fact]
    public void Withdraw_ExactBalance_LeavesZero()
    {
        var account = OpenGelAccount();
        account.Deposit(Gel(100));

        account.Withdraw(Gel(100));

        account.Balance.Should().Be(Gel(0));
    }

    [Fact]
    public void Withdraw_MoreThanBalance_Throws_AndChangesNothing()
    {
        var account = OpenGelAccount();
        account.Deposit(Gel(50));

        var act = () => account.Withdraw(Gel(50.01m));

        act.Should().Throw<DomainException>().WithMessage("*Insufficient*");
        account.Balance.Should().Be(Gel(50));
        account.Version.Should().Be(1, "the failed withdrawal must not add an event");
    }

    [Fact]
    public void Withdraw_WrongCurrency_Throws()
    {
        var account = OpenGelAccount();
        account.Deposit(Gel(50));

        var act = () => account.Withdraw(Usd(10));

        act.Should().Throw<DomainException>().WithMessage("*GEL*USD*");
    }

    // ---------- Closing ----------

    [Fact]
    public void Close_WithZeroBalance_ClosesAccount()
    {
        var account = OpenGelAccount();

        account.Close("customer request");

        account.Status.Should().Be(AccountStatus.Closed);
        account.UncommittedEvents.Last().Should().BeOfType<AccountClosed>();
    }

    [Fact]
    public void Close_WithMoneyLeft_Throws()
    {
        var account = OpenGelAccount();
        account.Deposit(Gel(0.01m));

        var act = () => account.Close();

        act.Should().Throw<DomainException>().WithMessage("*zero*");
        account.Status.Should().Be(AccountStatus.Open);
    }

    [Fact]
    public void Close_AlreadyClosed_Throws()
    {
        var account = OpenGelAccount();
        account.Close();

        var act = () => account.Close();

        act.Should().Throw<DomainException>().WithMessage("*closed*");
    }

    [Fact]
    public void ClosedAccount_RejectsDepositAndWithdraw()
    {
        var account = OpenGelAccount();
        account.Close();

        var deposit = () => account.Deposit(Gel(10));
        var withdraw = () => account.Withdraw(Gel(10));

        deposit.Should().Throw<DomainException>().WithMessage("*closed*");
        withdraw.Should().Throw<DomainException>().WithMessage("*closed*");
    }

    [Fact]
    public void Account_DepositWithdrawThenClose_FullLifecycle()
    {
        var account = OpenGelAccount();
        account.Deposit(Gel(100));
        account.Withdraw(Gel(100));

        account.Close();

        account.Status.Should().Be(AccountStatus.Closed);
        account.Version.Should().Be(3);
        account.UncommittedEvents.Select(e => e.GetType()).Should().Equal(
            typeof(AccountOpened),
            typeof(MoneyDeposited),
            typeof(MoneyWithdrawn),
            typeof(AccountClosed));
    }

}
