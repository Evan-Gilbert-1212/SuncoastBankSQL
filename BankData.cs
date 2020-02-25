using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

namespace SuncoastBank
{
  public partial class BankData : DbContext
  {
    public DbSet<User> Users { get; set; }

    public DbSet<Account> Accounts { get; set; }

    public DbSet<Transaction> Transactions { get; set; }

    public bool UserVerified(string userName, string password)
    {
      var userAccount = Users.FirstOrDefault(user => user.UserName == userName && user.Password == password);

      if (userAccount != null)
        return true;
      else
        return false;
    }

    public bool UserExists(string userName)
    {
      if (Users.Count(user => user.UserName == userName) > 0)
        return true;
      else
        return false;
    }

    public int GetUserID(string userName)
    {
      return Users.FirstOrDefault(user => user.UserName == userName).ID;
    }

    public void CreateUser(string userName, string password)
    {
      var newUser = new User();

      newUser.UserName = userName;
      newUser.Password = password;

      Users.Add(newUser);

      SaveChanges();
    }

    public void CreateAccounts(int userID)
    {
      var newCheckingAcct = new Account();

      newCheckingAcct.User_ID = userID;
      newCheckingAcct.AccountType = "checking";
      newCheckingAcct.AccountBalance = 0;

      Accounts.Add(newCheckingAcct);

      var newSavingsAcct = new Account();

      newSavingsAcct.User_ID = userID;
      newSavingsAcct.AccountType = "savings";
      newSavingsAcct.AccountBalance = 0;

      Accounts.Add(newSavingsAcct);

      SaveChanges();
    }

    public void DepositInAccount(int accountID, decimal amountToDeposit)
    {
      Accounts.FirstOrDefault(acc => acc.ID == accountID).AccountBalance += amountToDeposit;

      var depositTransaction = new Transaction();
      depositTransaction.Account_ID = accountID;
      depositTransaction.TransactionType = "Deposit";
      depositTransaction.TransactionAmount = amountToDeposit;

      Transactions.Add(depositTransaction);

      SaveChanges();
    }

    public bool WithdrawFromAccount(int accountID, decimal amountToWithdraw)
    {
      var accountBalance = Accounts.FirstOrDefault(acc => acc.ID == accountID).AccountBalance;

      if (accountBalance >= amountToWithdraw)
      {
        Accounts.FirstOrDefault(acc => acc.ID == accountID).AccountBalance -= amountToWithdraw;

        var withdrawalTransaction = new Transaction();
        withdrawalTransaction.Account_ID = accountID;
        withdrawalTransaction.TransactionType = "Withdrawal";
        withdrawalTransaction.TransactionAmount = amountToWithdraw;

        Transactions.Add(withdrawalTransaction);

        SaveChanges();

        return true;
      }
      else
      {
        return false;
      }
    }

    public bool TransferFromAccount(int userID, int transferFromAccountID, decimal amountToTransfer)
    {
      var transferToAccountID = Accounts.FirstOrDefault(acc => acc.ID != transferFromAccountID && acc.User_ID == userID).ID;

      var accountBalance = Accounts.FirstOrDefault(acc => acc.ID == transferFromAccountID).AccountBalance;

      if (accountBalance >= amountToTransfer)
      {
        Accounts.FirstOrDefault(acc => acc.ID == transferFromAccountID).AccountBalance -= amountToTransfer;
        Accounts.FirstOrDefault(acc => acc.ID == transferToAccountID).AccountBalance += amountToTransfer;

        var transferOutTransaction = new Transaction();
        transferOutTransaction.Account_ID = transferFromAccountID;
        transferOutTransaction.TransactionType = "Transfer Out";
        transferOutTransaction.TransactionAmount = amountToTransfer;


        Transactions.Add(transferOutTransaction);

        var transferInTransaction = new Transaction();
        transferInTransaction.Account_ID = transferToAccountID;
        transferInTransaction.TransactionType = "Transfer In";
        transferInTransaction.TransactionAmount = amountToTransfer;

        Transactions.Add(transferInTransaction);

        SaveChanges();

        return true;
      }
      else
      {
        return false;
      }
    }

    public void DisplayAccounts(int userID)
    {
      var accountsToDisplay = Accounts.Where(acc => acc.User_ID == userID).ToList();

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

    public void DisplayTransactionHistory(int accountID)
    {
      var historyToDisplay = Transactions.Where(trans => trans.Account_ID == accountID)
                                         .OrderBy(trans => trans.TransactionDate).ToList();

      Console.WriteLine($"Your {Accounts.FirstOrDefault(acc => acc.ID == accountID).AccountType} Account Transaction History");
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine("| Transaction      | Amount     | Transaction Date      |");
      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

      foreach (var trans in historyToDisplay)
      {
        var formatTransType = String.Format("{0,-16}", trans.TransactionType);
        var formatTransAmount = String.Format("{0,-10}", trans.TransactionAmount);
        var formatTransDate = String.Format("{0,-21}", trans.TransactionDate);

        Console.WriteLine($"| {formatTransType} | {formatTransAmount} | {formatTransDate} |");
      }

      Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
      Console.WriteLine();
    }


    public BankData()
    {
    }

    public BankData(DbContextOptions<BankData> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        //#warning To protect potentially sensitive information in your connection string, 
        //you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 
        //for guidance on storing connection strings.
        optionsBuilder.UseNpgsql("server=localhost;database=SuncoastBank");
      }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
  }
}
