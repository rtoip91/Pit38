﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table("DividendCalculations")]
    public  class DividendCalculationsEntity
    {
        [Key] public int Id { get; set; }

        public int PositionId { get; set; }
        public string InstrumentName { get; set; }

        public DateTime DateOfPayment { get; set; }

        public string Currency { get; set; }

        public decimal ExchangeRate { get; set; }

        public decimal DividendReceived { get; set; }

        public decimal DividendReceivedExchanged { get; set; }

        public decimal WithholdingTaxRate { get; set; }

        public decimal WithholdingTaxPaid { get; set; }

        public decimal WithholdingTaxRemain { get; set; }

        public string Country { get; set; }

        public override string ToString()
        {
            return $"Dywidenda za: {InstrumentName} | ID: {PositionId} | Kraj: {Country} |" +
                   $"\nData otrzymania: {DateOfPayment.ToShortDateString()} | Wartość : {DividendReceived} {Currency} |" +
                   $"\nKurs {Currency} z dnia poprzedniego: {ExchangeRate} PLN | Po przeliczeniu: {DividendReceivedExchanged} |" +
                   $"\nStawka podatku: {WithholdingTaxRate}% | Podatek zapłacony {WithholdingTaxPaid} PLN | Podatek do zapłaty {WithholdingTaxRemain} PLN |\n";
        }
    }
}
