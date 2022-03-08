﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table("SoldCryptoCalculations")]
    public class SoldCryptoEntity
    {
        [Key] public int Id { get; set; }
        public long PositionId { get; set; }
        public string Name { get; set; }
        public DateTime SellDate { get; set; }
        public decimal Units { get; set; }
        public decimal ValuePerUnit { get; set; }
        public decimal TotalValue { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal TotalExchangedValue { get; set; }

        public override string ToString()
        {
            return $"Operacja: {Name} | ID: {PositionId} |" +
                   $"\nIlość jednostek: {Units} |" +
                   $"\nData sprzedaży: {SellDate.ToShortDateString()} | Cena za jednostkę: {ValuePerUnit} {CurrencySymbol} |" +
                   $"\nWartość jednostek w dniu sprzedaży: {TotalValue} {CurrencySymbol} | Kurs {CurrencySymbol} z dnia poprzedniego: {ExchangeRate} PLN |" +
                   $"\nPo przeliczeniu: {TotalExchangedValue} PLN\n";
        }
    }
}
