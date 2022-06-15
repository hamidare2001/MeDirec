
using System;
using System.Threading.Tasks;
using MeDirect.ExchangeApi.Entities;
using MeDirect.ExchangeApi.Infrastructure;
using MeDirect.ExchangeApi.Models;
using MeDirect.ExchangeApi.Services;
using MeDirect.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using RateService.Models;

namespace MeDirect.Test
{
    public class ExchangeTest
    {
        private ExchangeExchangeService _exchangeExchangeService;
        private Mock<IUserService> _userService;
        private Mock<ICachingRentService> _mockICachingRentService;
        private Mock<IRateApi> _mockIRateApi;
        private CacheRate _cacheRate;
        private CacheUser _cacheUser;
        private MeDirectDbContext _context;

        [SetUp]
        public async Task Setup()
        {
            _userService = new Mock<IUserService>();
            _userService
                .Setup(c => c.LimitTradeInHour(It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));
            
            _userService
                .Setup(c => c.UpdateUserExchanges(It.IsAny<Exchange>()))
                .Returns(Task.FromResult(true));


            _mockICachingRentService = new Mock<ICachingRentService>();
            _mockICachingRentService
                .Setup(c => c.IsRateCreditable(It.IsAny<Trade>()))
                .Returns(Task.FromResult(true));

            _mockIRateApi = new Mock<IRateApi>();

            _context = await FillDataHelper.FillDataBase();
            var user = await _context.Users.FirstOrDefaultAsync();

            _cacheUser = new CacheUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Family = user.Family,
                Name = user.Name,
            };

            _cacheRate = new CacheRate
            {
                CashRateId = Guid.NewGuid(),
                CurrencyFrom = Currency.USD,
                CurrencyTo = Currency.EUR,
                Amount = 10000,
            };

            _exchangeExchangeService = new ExchangeExchangeService(_userService.Object,
                _mockICachingRentService.Object, _context, _mockIRateApi.Object);
        }

        [Test]
        public async Task Exchange_Success_Test()
        {
            await _exchangeExchangeService.ExchangeDone(new Trade
            {
                UserId = _cacheUser.Id,
                CurrencyFrom = _cacheRate.CurrencyFrom,
                CurrencyTo = _cacheRate.CurrencyTo,
                Amount = _cacheRate.Amount,
                CashRateId = _cacheRate.CashRateId
            });


            var updateUser = await _context.Users.Include(c => c.Exchanges)
                 .FirstOrDefaultAsync(c => c.Id == _cacheUser.Id);

            if (updateUser != null)
                Assert.That(updateUser.Exchanges.Count, Is.EqualTo(1));
            else
                Assert.That(false);
        }

        [Test]
        public void Exchange_Duplicate_currency_Test()
        {
            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await _exchangeExchangeService.ExchangeDone(new Trade
                {
                    CurrencyFrom = _cacheRate.CurrencyFrom,
                    CurrencyTo = _cacheRate.CurrencyFrom,
                });
            });

            if (ex != null)
                Assert.That(ex.Message, Is.EqualTo(Messages.CurrencyDuplicated));
        }


        [Test]
        public void Exchange_LimitInTrade_Test()
        {
            _userService
                .Setup(c => c.LimitTradeInHour(It.IsAny<Guid>()))
                .Returns(Task.FromResult(false));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await _exchangeExchangeService.ExchangeDone(new Trade
                {
                    UserId = _cacheUser.Id,
                    CurrencyFrom = _cacheRate.CurrencyFrom,
                    CurrencyTo = _cacheRate.CurrencyTo,
                });
            });

            if (ex != null)
                Assert.That(ex.Message, Is.EqualTo(Messages.LimitInTrade));
        }

        [Test]
        public void Exchange_RateExpired_Test()
        {
            _mockICachingRentService
                .Setup(c => c.IsRateCreditable(It.IsAny<Trade>()))
                .Returns(Task.FromResult(false));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await _exchangeExchangeService.ExchangeDone(new Trade
                {
                    UserId = _cacheUser.Id,
                    CurrencyFrom = _cacheRate.CurrencyFrom,
                    CurrencyTo = _cacheRate.CurrencyTo,
                });
            });

            if (ex != null)
                Assert.That(ex.Message, Is.EqualTo(Messages.RateExpired));
        }

    }
}
