using System;

namespace MeDirect.ExchangeApi.Models
{
    public record Trade : CacheRate
    {
        public Guid UserId { get; init; }
    }
}