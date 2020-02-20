using System;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;

namespace SuncoastBank
{
  public class AccountManager
  {
    public List<Account> userAccounts = new List<Account>();

    public List<Transaction> userTransactions = new List<Transaction>();

    public void LoadAccountsFromFile()
    {
      using (var reader = new StreamReader("Accounts.csv"))
      using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
      {
        userAccounts = csvReader.GetRecords<Account>().ToList();
      }
    }

    public void SaveAccountsToFile()
    {
      using (var writer = new StreamWriter("Accounts.csv"))
      using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
      {
        csvWriter.WriteRecords(userAccounts);
        writer.Flush();
      }
    }

    public void LoadTransactionsFromFile()
    {
      using (var reader = new StreamReader("Transactions.csv"))
      using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
      {
        userTransactions = csvReader.GetRecords<Transaction>().ToList();
      }
    }

    public void SaveTransactionsToFile()
    {
      using (var writer = new StreamWriter("Transactions.csv"))
      using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
      {
        csvWriter.WriteRecords(userTransactions);
        writer.Flush();
      }
    }

    public void DepositInAccount(string userName, string accountType, decimal amountToDeposit)
    {
      userAccounts.Find(account => account.UserName == userName && account.AccountType == accountType).AccountBalance += amountToDeposit;

      var depositTransaction = new Transaction();
      depositTransaction.UserName = userName;
      depositTransaction.AccountType = accountType;
      depositTransaction.TransactionType = "Deposit";
      depositTransaction.TransactionAmount = amountToDeposit;

      userTransactions.Add(depositTransaction);

      SaveAccountsToFile();
      SaveTransactionsToFile();

      Console.WriteLine();
      Console.WriteLine($"Deposit to {accountType} account saved successfully!");
      Console.WriteLine();
    }

    public void WithdrawFromAccount(string userName, string accountType, decimal amountToWithdraw)
    {
      userAccounts.Find(account => account.UserName == userName && account.AccountType == accountType).AccountBalance -= amountToWithdraw;

      var withdrawalTransaction = new Transaction();
      withdrawalTransaction.UserName = userName;
      withdrawalTransaction.AccountType = accountType;
      withdrawalTransaction.TransactionType = "Withdrawal";
      withdrawalTransaction.TransactionAmount = amountToWithdraw;

      userTransactions.Add(withdrawalTransaction);

      SaveAccountsToFile();
      SaveTransactionsToFile();

      Console.WriteLine();
      Console.WriteLine($"Withdrawal from {accountType} account saved successfully!");
      Console.WriteLine();
    }

    public void TransferFromAccount(string userName, string accountType, decimal amountToTransfer)
    {
      var transferToAccount = userAccounts.Find(account => account.UserName == userName && account.AccountType != accountType).AccountType;

      userAccounts.Find(account => account.UserName == userName && account.AccountType == accountType).AccountBalance -= amountToTransfer;
      userAccounts.Find(account => account.UserName == userName && account.AccountType == transferToAccount).AccountBalance += amountToTransfer;

      var transferOutTransaction = new Transaction();
      transferOutTransaction.UserName = userName;
      transferOutTransaction.AccountType = accountType;
      transferOutTransaction.TransactionType = "Transfer Out";
      transferOutTransaction.TransactionAmount = amountToTransfer;

      userTransactions.Add(transferOutTransaction);

      var transferInTransaction = new Transaction();
      transferInTransaction.UserName = userName;
      transferInTransaction.AccountType = transferToAccount;
      transferInTransaction.TransactionType = "Transfer In";
      transferInTransaction.TransactionAmount = amountToTransfer;

      userTransactions.Add(transferInTransaction);

      SaveAccountsToFile();
      SaveTransactionsToFile();

      Console.WriteLine();
      Console.WriteLine($"Transfer from {accountType} account to {transferToAccount} account saved successfully!");
      Console.WriteLine();
    }

    public void DisplayTransactionHistory(string userName, string accountToView)
    {
      var historyToDisplay = userTransactions.Where(trans => trans.UserName == userName && trans.AccountType == accountToView)
                                         .OrderBy(trans => trans.TransactionDate).ToList();

      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine("| Account Type | Transaction | Amount | Transaction Date     |");
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

      foreach (var trans in historyToDisplay)
      {
        var formatAccountType = String.Format("{0,-12}", trans.AccountType);
        var formatTransType = String.Format("{0,-11}", trans.TransactionType);
        var formatTransAmount = String.Format("{0,-6}", trans.TransactionAmount);
        var formatTransDate = String.Format("{0,-16}", trans.TransactionDate);

        Console.WriteLine($"| {formatAccountType} | {formatTransType} | {formatTransAmount} | {formatTransDate} |");
      }

      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine();
    }
  }
}