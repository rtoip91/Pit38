﻿using System.Collections.Generic;
using System.Linq;
using Database.DataAccess.Interfaces;
using Database.Entities.InMemory;
using Database.Repository;

namespace Database.DataAccess
{
    public sealed class TransactionReportsDataAccess : ITransactionReportsDataAccess
    {
        private readonly IDataRepository _importRepository;

        public TransactionReportsDataAccess(IDataRepository importRepository)
        {
            _importRepository = importRepository;
        }

        public void AddTransactionReports(IList<TransactionReportEntity> transactionReports)
        {
            foreach (var transactionReport in transactionReports)
            {
                _importRepository.TransactionReports.Add(transactionReport);
            }
        }

        public IList<TransactionReportEntity> GetUnsoldCryptoTransactions(string cryptoName)
        {
            return _importRepository.TransactionReports.Where(c =>
                c.Type.ToLower().Contains("Otwarta pozycja".ToLower())
                && c.Details.ToLower().Contains($"{cryptoName.ToLower()}/")
                && c.ClosedPosition == null).ToList();
        }
    }
}