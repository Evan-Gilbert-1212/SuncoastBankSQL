using System;

namespace SuncoastBank
{
  public class Transaction
  {
    public int ID { get; set; }

    public int Account_ID { get; set; }

    public string TransactionType { get; set; }

    public decimal TransactionAmount { get; set; }

    public DateTime TransactionDate { get; set; } = DateTime.Now;
  }
}