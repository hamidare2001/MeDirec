using System;
using RateService.Models;

namespace MeDirect.ExchangeApi.Models
{
    public record CacheRate
    {
        public Guid CashRateId { get; init; }
        public Currency CurrencyFrom { get; init; }
        public Currency CurrencyTo { get; init; }
        public double Amount { get; init; }
    }
}
