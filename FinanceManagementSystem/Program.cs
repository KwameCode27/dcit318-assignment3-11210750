﻿using System;
using System.Collections.Generic;

public record Transaction(int Id, DateTime Date, decimal Amount, string Category);

public interface ITransactionProcessor
{
    void Process(Transaction transaction);
}

public class BankTransferProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing bank transfer of {transaction.Amount} for category {transaction.Category} on {transaction.Date:d}.");
    }
}

public class MobileMoneyProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing mobile money transfer of {transaction.Amount} for category {transaction.Category} on {transaction.Date:d}.");
    }
}

public class CryptoWalletProcessor : ITransactionProcessor
{
    public void Process(Transaction transaction)
    {
        Console.WriteLine($"Processing crypto transaction of {transaction.Amount} for category {transaction.Category} on {transaction.Date:d}.");
    }
}

public class Account
{
    public string AccountName { get; set; }
    public decimal Balance { get; protected set; }
    
    public Account(string accountName, decimal initialBalance)
    {
        AccountName = accountName;
        Balance = initialBalance;
    }

    public virtual void ApplyTransaction(Transaction transaction)
    {
            Balance -= transaction.Amount;
       
    }
}


public sealed class SavingsAccount : Account
{
    public SavingsAccount(string accountName, decimal initialBalance) 
        : base(accountName, initialBalance) { }
    public override void ApplyTransaction(Transaction transaction)
    {
        if (transaction.Amount < 0 && Balance + transaction.Amount < 0)
        {
           Console.WriteLine("Insufficient funds for this transaction.");
        }
        else
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"SavingsAccount '{AccountName}' new balance: {Balance}");
        }
    }
}


public class FinanceApp
{
    public List<Transaction> _transactions { get; set; }

    public void Run()
    {
        SavingsAccount savingsAccount = new SavingsAccount("My Savings", 10000);

        Transaction grocery = new Transaction(1, DateTime.Now, 150, "Groceries");
        BankTransferProcessor bankTransferProcessor = new BankTransferProcessor();
        bankTransferProcessor.Process(grocery);

        Transaction utilities = new Transaction(2, DateTime.Now, 300, "Utilities");
        MobileMoneyProcessor mobileMoneyProcessor = new MobileMoneyProcessor();
        mobileMoneyProcessor.Process(utilities);

        Transaction entertainment = new Transaction(3, DateTime.Now, 400, "Entertainment");
        CryptoWalletProcessor cryptoWalletProcessor = new CryptoWalletProcessor();
        cryptoWalletProcessor.Process(entertainment);

        savingsAccount.ApplyTransaction(grocery);
        savingsAccount.ApplyTransaction(utilities);
        savingsAccount.ApplyTransaction(entertainment);


        _transactions.Add(grocery);
        _transactions.Add(utilities);
        _transactions.Add(entertainment);

    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        FinanceApp financeApp = new FinanceApp();
        financeApp._transactions = new List<Transaction>();
        financeApp.Run();
        Console.WriteLine("All transactions processed successfully.");
    }
}   