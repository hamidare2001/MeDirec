using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using RateService.Models;

namespace MeDirect.ExchangeApi.Models
{
    public class RateCurrency
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        public Currency CurrencyBase { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        [JsonPropertyName("rates")]
        public Dictionary<Currency, double> Rates { get; set; } = new Dictionary<Currency, double>();
    }
}
