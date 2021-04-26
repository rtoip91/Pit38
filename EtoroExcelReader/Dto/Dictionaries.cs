﻿using System.Collections.Generic;

namespace EtoroExcelReader.Dto
{
    public static class Dictionaries
    {
        public static Dictionary<string, string> StockExchangesDictionary = new Dictionary<string, string>
        {
            {"ZU","Szwajcaria"},
            {"DE","Niemcy" },
            {"L", "Wielka Brytania" },
            {"PA", "Francja" },
            {"MC", "Hiszpania" },
            {"MI", "Włochy" },
            {"OL", "Norwegia" },
            {"ST", "Szwecja" },
            {"CO", "Dania" },
            {"HE", "Finlandia" },
            {"HK","Hong Kong" },
            {"LSB", "Portugalia" },
            {"BR", "Belgia" },
            {"NV", "Holandia" }
        };

        public static Dictionary<string, string> CryptoCurrenciesDictionary = new Dictionary<string, string>
        {
            {"BTC", "Bitcoin" },
            {"ETH","Ethereum" },
            {"BCH","Bitcoin Cash" },
            {"XRP", "Ripple" },
            {"DASH", "Dash" },
            {"LTC", "Litecoin" },
            {"ETC", "Ethereum CLassic" },
            {"ADA", "Cardano" },
            {"MIOTA", "IOTA" },
            {"XLM", "Stellar" },
            {"EOS", "EOS" },
            {"NEO", "NEO" },
            {"TRX", "TRON" },
            {"ZEC", "ZCASH" },
            {"BNB", "Binance Coin" },
            {"XTZ", "Tezos" },
            {"Link", "ChainLink" },
            {"UNI", "Uniswap" }
        };
    }
}
