using MiniBank.Accounts.Domain.Accounts.Events;
using MiniBank.Accounts.Domain.Common;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace MiniBank.Accounts.Domain.Accounts;

public enum AccountStatus
{
    Open,
    Closed
}

public sealed class Account : AggregateRoot<AccountId>
{
    public CustomerId OwnerId { get; private set; }
    public Currency Currency { get; private set; }
    public Money Balance { get; private set; }
    public AccountStatus Status { get; private set; }

    public Account() { }

    public static Account Open(AccountId id, CustomerId ownerId, Currency currency)
    {
        if (string.IsNullOrWhiteSpace(ownerId.Value))
            throw new DomainException("An Account must have an owner.");

        var account = new Account();
        account.Raise(new AccountOpened { AccountId = id, OwnerId = ownerId, Currency = currency });
        return account;
    }

    public void Deposit(Money amount, string? description = null)
    {
        EnsureOpen();
        EnsurePositive(amount);
        EnsureCurrency(amount);

        Raise(new MoneyDeposited { AccountId = Id, Amount = amount, Description = description });
    }

    public void Withdraw(Money amount, string? description = null)
    {
        EnsureOpen();
        EnsurePositive(amount);
        EnsureCurrency(amount);
        EnsureSufficientFunds(amount);

        Raise(new MoneyWithdrawn { AccountId = Id, Amount = amount, Description = description });
    }

    public void Close(string? reason = null)
    {
        EnsureOpen();
        if (!Balance.IsZero)
            throw new DomainException("Account balance must be zero before closing.");

        Raise(new AccountClosed { AccountId = Id, Reason = reason });
    }

    private void EnsureOpen()
    {
        if (Status == AccountStatus.Closed)
            throw new DomainException("Account is closed.");
    }

    private static void EnsurePositive(Money amount)
    {
        if (!amount.IsPositive)
            throw new DomainException("Amount must be positive.");
    }

    private void EnsureCurrency(Money amount)
    {
        if (amount.Currency != Currency)
            throw new DomainException($"Account is in {Currency}; got {amount.Currency}.");
    }

    private void EnsureSufficientFunds(Money amount)
    {
        if (amount > Balance)
            throw new DomainException($"Insufficient funds: balance {Balance}, requested {amount}.");
    }

    protected override void When(IDomainEvent @event)
    {
        switch (@event)
        {
            case AccountOpened e:
                Id = e.AccountId;
                OwnerId = e.OwnerId;
                Currency = e.Currency;
                Balance = Money.Zero(e.Currency);
                Status = AccountStatus.Open;
                break;

            case MoneyDeposited e:
                Balance += e.Amount;
                break;

            case MoneyWithdrawn e:
                Balance -= e.Amount;
                break;

            case AccountClosed:
                Status = AccountStatus.Closed;
                break;

            default:
                throw new InvalidOperationException($"Unknown event {@event.GetType().Name} for Account.");
        }
    }
}
