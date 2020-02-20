using System;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Linq;

namespace SuncoastBank
{
  class Program
  {
    static void Main(string[] args)
    {
      var userName = "Evan";

      Console.Clear();
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine("Welcome to Suncoast Bank!");
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine();
      Console.WriteLine($"User Logged In: {userName}");
      Console.WriteLine();

      var accountManager = new AccountManager();

      accountManager.LoadAccountsFromFile();
      accountManager.LoadTransactionsFromFile();

      Console.WriteLine("Here is your Account Summary:");
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

      foreach (var account in accountManager.userAccounts)
      {
        Console.WriteLine($"{account.AccountType} Account : Balance of {account.AccountBalance}");
      }

      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine();

      var isRunning = true;

      while (isRunning)
      {
        Console.WriteLine("What would you like to do?");
        Console.WriteLine("(DEPOSIT) money into an account");
        Console.WriteLine("(WITHDRAW) money from an account");
        Console.WriteLine("(TRANSFER) money between your accounts");
        Console.WriteLine("(VIEW) the transaction history on an account");
        Console.WriteLine("(EXIT) Suncoast bank program");

        var userResponse = Console.ReadLine().ToLower();

        while (userResponse != "deposit" && userResponse != "withdraw" && userResponse != "transfer" &&
               userResponse != "view" && userResponse != "exit")
        {
          Console.WriteLine("Please enter a valid response. Valid responses are:");
          Console.WriteLine("(DEPOSIT), (WITHDRAW), (TRANSFER), (VIEW), (EXIT)");

          userResponse = Console.ReadLine().ToLower();
        }

        switch (userResponse)
        {
          case "deposit":
            Console.WriteLine();
            Console.WriteLine("Which account would you like to deposit to? (CHECKING) or (SAVINGS)?");

            var depositAccount = Console.ReadLine().ToLower();

            while (depositAccount != "checking" && depositAccount != "savings")
            {
              Console.WriteLine("Please enter a valid account type. Valid account types are (CHECKING) and (SAVINGS).");

              depositAccount = Console.ReadLine().ToLower();
            }

            Console.WriteLine();
            Console.WriteLine($"How much would you like to deposit in your {depositAccount} account?");

            var depositAmount = Console.ReadLine();
            decimal decDepositAmount;

            while (!decimal.TryParse(depositAmount, out decDepositAmount))
            {
              Console.WriteLine("Please enter a valid amount to deposit. Format should be '0.00'.");

              depositAmount = Console.ReadLine();
            }

            accountManager.DepositInAccount(userName, depositAccount, decDepositAmount);

            break;
          case "withdraw":
            Console.WriteLine();
            Console.WriteLine("Which account would you like to withdraw from? (CHECKING) or (SAVINGS)?");

            var withdrawalAccount = Console.ReadLine().ToLower();

            while (withdrawalAccount != "checking" && withdrawalAccount != "savings")
            {
              Console.WriteLine("Please enter a valid account type. Valid account types are (CHECKING) and (SAVINGS).");

              withdrawalAccount = Console.ReadLine().ToLower();
            }

            Console.WriteLine();
            Console.WriteLine($"How much would you like to withdraw from your {withdrawalAccount} account?");

            var withdrawalAmount = Console.ReadLine();
            decimal decWithdrawAmount;

            while (!decimal.TryParse(withdrawalAmount, out decWithdrawAmount))
            {
              Console.WriteLine("Please enter a valid amount to deposit. Format should be '0.00'.");

              withdrawalAmount = Console.ReadLine();
            }

            accountManager.WithdrawFromAccount(userName, withdrawalAccount, decWithdrawAmount);

            break;
          case "transfer":
            Console.WriteLine();
            Console.WriteLine("Which account would you like to transfer from? (CHECKING) or (SAVINGS)?");

            var transferFromAccount = Console.ReadLine().ToLower();

            while (transferFromAccount != "checking" && transferFromAccount != "savings")
            {
              Console.WriteLine("Please enter a valid account type. Valid account types are (CHECKING) and (SAVINGS).");

              transferFromAccount = Console.ReadLine().ToLower();
            }

            Console.WriteLine();
            Console.WriteLine($"How much would you like to transfer from your {transferFromAccount} account?");

            var transferFromAmount = Console.ReadLine();
            decimal decTransferFromAmount;

            while (!decimal.TryParse(transferFromAmount, out decTransferFromAmount))
            {
              Console.WriteLine("Please enter a valid amount to deposit. Format should be '0.00'.");

              transferFromAmount = Console.ReadLine();
            }

            accountManager.TransferFromAccount(userName, transferFromAmount, decTransferFromAmount);
            break;
          case "view":
            Console.WriteLine();
            Console.WriteLine("Which account would you like to view transactions from? (CHECKING) or (SAVINGS)?");

            var accountToView = Console.ReadLine().ToLower();

            while (accountToView != "checking" && accountToView != "savings")
            {
              Console.WriteLine("Please enter a valid account type. Valid account types are (CHECKING) and (SAVINGS).");

              accountToView = Console.ReadLine().ToLower();
            }

            accountManager.DisplayTransactionHistory(userName, accountToView);

            break;
          case "exit":
            isRunning = false;
            break;
        }
      }

      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine("Thank you for using Suncoast Bank! Goodbye!");
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
    }
  }
}
