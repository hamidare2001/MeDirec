using System;
using RateService.Models;

namespace MeDirect.ExchangeApi.Models
{
    public record GetRateModel
    {
        public Guid UserId { get; init; }
        public Currency CurrencyFrom { get; init; }
        public Currency CurrencyTo { get; init; }
    }
}