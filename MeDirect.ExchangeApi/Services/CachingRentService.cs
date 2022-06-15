using System;
using System.Threading.Tasks;
using MeDirect.ExchangeApi.Infrastructure;
using MeDirect.ExchangeApi.Models;
using Microsoft.Extensions.Caching.Distributed;
using RateService.Models;

namespace MeDirect.ExchangeApi.Services
{
    public interface ICachingRentService
    {
        Task<CacheRate> GetRate(string key);
        Task<CacheRate> GetRate(Currency fromCurrency, Currency toCurrency);

        Task<bool> IsRateCreditable(CacheRate cacheRate);

        Task<bool> InsertRate(CacheRate cacheRate);
    }

    public class CachingRentService: ICachingRentService
    {
        private readonly ICacheProvider _cacheProvider;

        private double CacheTimeToLive = 30;

        public CachingRentService(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public async Task<CacheRate> GetRate(string key)
        {
            return await _cacheProvider.GetFromCache<CacheRate>(key);
        }
        public async Task<CacheRate> GetRate(Currency fromCurrency, Currency toCurrency)
        {
            var key = CreateKey(fromCurrency, toCurrency);
            return await GetRate(key);
        }
        public async Task<bool> IsRateCreditable(CacheRate cacheRate)
        {
            var key = CreateKey(cacheRate.CurrencyFrom, cacheRate.CurrencyTo);
            var cache = await _cacheProvider.GetFromCache<CacheRate>(key);

            return cache != null && cache.CashRateId == cacheRate.CashRateId;

        }

        public async Task<bool> InsertRate(CacheRate cacheRate)
        {
            var cacheEntryOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheTimeToLive));

            var key = CreateKey(cacheRate.CurrencyFrom, cacheRate.CurrencyTo);
            await _cacheProvider.SetCache(key, cacheRate, cacheEntryOptions);

            return true;
        }


        #region Private Methods
        static string CreateKey(Currency currencyFrom, Currency currencyTo) => $"{currencyFrom}{currencyTo}";
        #endregion
    }
}
