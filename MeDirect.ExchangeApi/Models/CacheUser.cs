using System;
using System.Collections.Generic;
using MeDirect.ExchangeApi.Entities;

namespace MeDirect.ExchangeApi.Models
{
    public record CacheUser
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Family { get; init; }
        public string UserName { get; init; }
        public virtual List<Exchange> Exchanges { get; init; } = new List<Exchange>();

    }
}
