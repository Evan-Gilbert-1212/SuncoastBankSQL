using System;
using System.Linq;

namespace SuncoastBank
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.Clear();
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine("Welcome to Suncoast Bank!");
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine();

      var accountManager = new AccountManager();

      accountManager.LoadUsersFromFile();
      accountManager.LoadAccountsFromFile();
      accountManager.LoadTransactionsFromFile();

      var userName = "";
      var password = "";

      RequestLoginDetails(accountManager, out userName, out password);

      while (!accountManager.AccountVerified(userName, password))
      {
        Console.WriteLine();
        Console.WriteLine("Invalid User Name or password. Please try again.");
        Console.WriteLine();

        RequestLoginDetails(accountManager, out userName, out password);
      }

      MyClearConsole(accountManager, userName);

      var isRunning = true;

      while (isRunning)
      {
        Console.WriteLine("What would you like to do?");
        Console.WriteLine("(DEPOSIT) money into an account");
        Console.WriteLine("(WITHDRAW) money from an account");
        Console.WriteLine("(TRANSFER) money between your accounts");
        Console.WriteLine("(VIEW) the transaction history on an account");
        Console.WriteLine("(EXIT) Suncoast Bank program");

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

            MyClearConsole(accountManager, userName);

            Console.WriteLine($"Deposit to {depositAccount} account saved successfully!");
            Console.WriteLine();

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

            MyClearConsole(accountManager, userName);

            Console.WriteLine($"Withdrawal from {withdrawalAccount} account saved successfully!");
            Console.WriteLine();

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

            accountManager.TransferFromAccount(userName, transferFromAccount, decTransferFromAmount);

            MyClearConsole(accountManager, userName);

            Console.WriteLine($"Transfer from {transferFromAccount} account saved successfully!");
            Console.WriteLine();

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

            MyClearConsole(accountManager, userName);

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

    static void RequestLoginDetails(AccountManager accountManager, out string userName, out string password)
    {
      Console.WriteLine("Please enter your User Name:");

      //Need more validation on this input
      //need to make sure empty is not an acceptable user
      userName = Console.ReadLine();

      if (!accountManager.AccountExists(userName))
      {
        //Create account/ask for password
        Console.WriteLine("You have not set up your account yet. Lets do that now!");
        Console.WriteLine("Please enter a password for your account:");

        var newPassword = Console.ReadLine();

        accountManager.CreateUser(userName, newPassword);

        accountManager.CreateAccounts(userName);

        password = newPassword;
      }
      else
      {
        //Build logic for reading password
        Console.WriteLine();
        Console.WriteLine("Please enter your password:");

        password = "";
        ConsoleKeyInfo keystroke;

        do
        {
          keystroke = Console.ReadKey(true);

          // Backspace Should Not Work
          if (keystroke.Key != ConsoleKey.Backspace && keystroke.Key != ConsoleKey.Enter)
          {
            password += keystroke.KeyChar;
            Console.Write("*");
          }
          else if (keystroke.Key == ConsoleKey.Backspace)
          {
            Console.Write("\b");
          }
        }
        while (keystroke.Key != ConsoleKey.Enter);
      }
    }

    static void MyClearConsole(AccountManager accountManager, string userName)
    {
      Console.Clear();
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine("Welcome to Suncoast Bank!");
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine();
      Console.WriteLine($"Welcome, {userName}!");
      Console.WriteLine();

      accountManager.DisplayAccounts(userName);
    }
  }
}
