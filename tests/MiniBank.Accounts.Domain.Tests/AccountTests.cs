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

}
