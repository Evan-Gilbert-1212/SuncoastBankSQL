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
    public List<User> userList = new List<User>();

    public List<Account> userAccounts = new List<Account>();

    public List<Transaction> userTransactions = new List<Transaction>();

    public void LoadUsersFromFile()
    {
      using (var reader = new StreamReader("Users.csv"))
      using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
      {
        userList = csvReader.GetRecords<User>().ToList();
      }
    }

    public void SaveUsersToFile()
    {
      using (var writer = new StreamWriter("Users.csv"))
      using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
      {
        csvWriter.WriteRecords(userList);
        writer.Flush();
      }
    }

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

    public bool AccountVerified(string userName, string password)
    {
      if (userList.Count(user => user.UserName == userName && user.Password == password) > 0)
        return true;
      else
        return false;
    }

    public bool AccountExists(string userName)
    {
      if (userList.Count(user => user.UserName == userName) > 0)
        return true;
      else
        return false;
    }

    public void CreateUser(string userName, string password)
    {
      var newUser = new User();

      newUser.UserName = userName;
      newUser.Password = password;

      userList.Add(newUser);

      SaveUsersToFile();
    }

    public void CreateAccounts(string userName)
    {
      var newCheckingAcct = new Account();

      newCheckingAcct.UserName = userName;
      newCheckingAcct.AccountType = "checking";
      newCheckingAcct.AccountBalance = 0;

      userAccounts.Add(newCheckingAcct);

      var newSavingsAcct = new Account();

      newSavingsAcct.UserName = userName;
      newSavingsAcct.AccountType = "savings";
      newSavingsAcct.AccountBalance = 0;

      userAccounts.Add(newSavingsAcct);

      SaveAccountsToFile();
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
    }

    public bool WithdrawFromAccount(string userName, string accountType, decimal amountToWithdraw)
    {
      var accountBalance = userAccounts.Find(account => account.UserName == userName && account.AccountType == accountType).AccountBalance;

      if (accountBalance >= amountToWithdraw)
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

        return true;
      }
      else
      {
        return false;
      }
    }

    public bool TransferFromAccount(string userName, string accountType, decimal amountToTransfer)
    {
      var transferToAccount = userAccounts.Find(account => account.UserName == userName && account.AccountType != accountType).AccountType;

      var accountBalance = userAccounts.Find(account => account.UserName == userName && account.AccountType == accountType).AccountBalance;

      if (accountBalance >= amountToTransfer)
      {
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

        return true;
      }
      else
      {
        return false;
      }
    }

    public void DisplayAccounts(string userName)
    {
      var accountsToDisplay = userAccounts.Where(account => account.UserName == userName).ToList();

      Console.WriteLine("Your Account Summary");
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine("| Account Type | Balance     |");
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

      foreach (var account in accountsToDisplay)
      {
        var formatAccountType = String.Format("{0,-12}", account.AccountType);
        var formatAccountBalance = String.Format("{0,-11}", account.AccountBalance);

        Console.WriteLine($"| {formatAccountType} | {formatAccountBalance} |");
      }

      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine();
    }

    public void DisplayTransactionHistory(string userName, string accountToView)
    {
      var historyToDisplay = userTransactions.Where(trans => trans.UserName == userName && trans.AccountType == accountToView)
                                         .OrderBy(trans => trans.TransactionDate).ToList();

      Console.WriteLine($"Your {accountToView} Account Transaction History");
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine("| Account Type | Transaction      | Amount     | Transaction Date      |");
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

      foreach (var trans in historyToDisplay)
      {
        var formatAccountType = String.Format("{0,-12}", trans.AccountType);
        var formatTransType = String.Format("{0,-16}", trans.TransactionType);
        var formatTransAmount = String.Format("{0,-10}", trans.TransactionAmount);
        var formatTransDate = String.Format("{0,-21}", trans.TransactionDate);

        Console.WriteLine($"| {formatAccountType} | {formatTransType} | {formatTransAmount} | {formatTransDate} |");
      }

      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine();
    }
  }
}