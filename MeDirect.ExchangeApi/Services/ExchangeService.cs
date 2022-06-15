using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeDirect.ExchangeApi.Entities;
using MeDirect.ExchangeApi.Infrastructure;
using MeDirect.ExchangeApi.Models;
using RateService.Models;

namespace MeDirect.ExchangeApi.Services
{
    public interface IExchangeService
    {
        Task<bool> ExchangeDone(Trade trade);
        Task<CacheRate> GetLastRate(GetRateModel getRateModel);
    }


    public class ExchangeExchangeService : IExchangeService
    {
        private readonly IUserService _userService;
        private readonly MeDirectDbContext _context;
        private readonly IRateApi _rateApi;
        private readonly ICachingRentService _caching;

        public ExchangeExchangeService(IUserService userService, ICachingRentService caching,
            MeDirectDbContext context,IRateApi rateApi)
        {
            _userService = userService;
            _caching = caching;
            _context = context;
            _rateApi = rateApi;
        }

        public async Task<bool> ExchangeDone(Trade trade)
        {
            if (trade.CurrencyFrom == trade.CurrencyTo)
                throw new Exception(Messages.CurrencyDuplicated);

            var canTrade = await _userService.LimitTradeInHour(trade.UserId);
            if (!canTrade)
            {
                throw new Exception(Messages.LimitInTrade);
            }
            

            var cacheRate = await _caching.IsRateCreditable(trade);
            if (!cacheRate )
                throw new Exception(Messages.RateExpired);
             

            var result = await SaveExchange(trade);
            return result;
        }



        public async Task<CacheRate> GetLastRate(GetRateModel getRateModel)
        {
            if (getRateModel.CurrencyFrom == getRateModel.CurrencyTo)
                throw new Exception(Messages.CurrencyDuplicated);

            var cacheRate = await _caching.GetRate(getRateModel.CurrencyFrom, getRateModel.CurrencyTo);
            if (cacheRate != null)
                return cacheRate;
             
            var rate = await _rateApi.GetRate(getRateModel.CurrencyFrom, new List<Currency> { getRateModel.CurrencyTo });

            if (rate == null || rate.Rates.Count == 0)
                return null;

            var newRate = new CacheRate
            {
                CashRateId = Guid.NewGuid(),
                CurrencyFrom = getRateModel.CurrencyFrom,
                CurrencyTo = getRateModel.CurrencyTo,
                Amount = rate.Rates.First().Value,
            };

            await _caching.InsertRate(newRate);

            return newRate;
        }

        #region Private Methods
        private async Task<bool> SaveExchange(Trade trade)
        {
           

            var exchange = new Exchange
            {
                Amount = trade.Amount,
                CurrencyFrom = trade.CurrencyFrom,
                CurrencyTo = trade.CurrencyTo,
                ExchangeDate = DateTime.Now,
                UserId = trade.UserId,
            };

            var result = await SaveExchangeInDataBase(exchange);
            if (result)
                return await _userService.UpdateUserExchanges(exchange);

            return false;

            // or return result && await _userService.UpdateUserExchanges(exchange);
        }

        private async Task<bool> SaveExchangeInDataBase(Exchange exchange)
        {
            _context.Exchanges.Add(exchange);

            return await _context.SaveChangesAsync() > 0;
        } 
        #endregion

    }
}
