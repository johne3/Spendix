using System;
using System.Collections.Generic;
using System.Text;
using Spendix.Core.Entities;
using System.Linq;
using System.Threading.Tasks;
using Spendix.Core.Models.BankAccountTransactionRepoModels;
using Microsoft.EntityFrameworkCore;

namespace Spendix.Core.Repos
{
    public class BankAccountTransactionRepo : EntityRepo<BankAccountTransaction>
    {
        public BankAccountTransactionRepo(SpendixDbContext spendixDbContext) : base(spendixDbContext)
        {
        }

        public Task<List<BankAccountTransaction>> FindByBankAccountAsync(BankAccount bankAccount)
        {
            var q = from bat in DataContext.BankAccountTransactions//.Include(x => x.BankAccountTransactionCategory)
                    where bat.BankAccountId == bankAccount.BankAccountId
                    select bat;

            return q.ToListAsync();
        }

        public Task<List<BankAccountTransactionBalanceModel>> FindBankAccountTransactionBalanceModelsByBankAccount(BankAccount bankAccount)
        {
            var sql = "SELECT bat.BankAccountTransactionId, " +
                      "bat.Amount + SUM(bat.Amount) OVER(PARTITION BY BankAccountId " +
                      "ORDER BY bat.BankAccountId, bat.TransactionDate, bat.TransactionEnteredDateUtc " +
                      "ROWS BETWEEN UNBOUNDED PRECEDING AND 1 PRECEDING) AS CurrentBalance " +
                      "FROM BankAccountTransaction bat " +
                      "WHERE bat.BankAccountId = {0} " +
                      "ORDER BY bat.BankAccountId, bat.TransactionDate, bat.TransactionEnteredDateUtc";

            var q = from m in DataContext.BankAccountTransactionBalanceModels.FromSql(sql, bankAccount.BankAccountId)
                    select m;

            return q.ToListAsync();
        }
    }
}