using System;

namespace SuncoastBank
{
  public class Transaction
  {
    public string UserName { get; set; }

    public string AccountType { get; set; }

    public string TransactionType { get; set; }

    public decimal TransactionAmount { get; set; }

    public DateTime TransactionDate { get; set; } = DateTime.Now;
  }
}