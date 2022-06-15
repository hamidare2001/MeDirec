using System;
using System.Threading.Tasks;
using MeDirect.ExchangeApi.Entities;
using MeDirect.ExchangeApi.Infrastructure;
using MeDirect.ExchangeApi.Models;
using MeDirect.ExchangeApi.Services;
using MeDirect.Test.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace MeDirect.Test
{
    public class LimitTradeInHourTest
    { 
        private UserService _service;
        private Mock<ICacheUserService> _mockICacheUserService; 
        private MeDirectDbContext _context;
        private CacheUser _cacheUser;

        [SetUp]
        public async Task Setup()
        { 
            _mockICacheUserService = new Mock<ICacheUserService>();
            _context = await FillDataHelper.FillDataBase();

            var user = await _context.Users.FirstOrDefaultAsync();

            _cacheUser = new CacheUser
            {
                Id = user.Id,
                UserName = user.UserName,
                Family = user.Family,
                Name = user.Name,
            };

            _service = new UserService(_context, _mockICacheUserService.Object );
        }


        [Test]
        public void EmptyUserId_LimitTradeInHour_Test()
        {
            _mockICacheUserService
                .Setup(c => c.GetCachedUser(Guid.Empty))
                .Returns(Task.FromResult<CacheUser>(null));

            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await _service.LimitTradeInHour(Guid.Empty);

            });


            if (ex != null) 
                Assert.That(ex.Message, Is.EqualTo(Messages.IsNotLogin));
        }

        [Test]
        public void NotExistUserId_LimitTradeInHour_Test()
        {
            _mockICacheUserService
                .Setup(c => c.GetCachedUser(It.IsAny<Guid>()))
                .Returns(Task.FromResult<CacheUser>(null));
            
            var ex = Assert.ThrowsAsync<Exception>(async () =>
            {
                await _service.LimitTradeInHour(Guid.Empty);
            });

            if (ex != null)
                Assert.That(ex.Message, Is.EqualTo(Messages.IsNotLogin));

        }

        [Test]
        public async Task Valid_LimitTradeInHour_Test()
        {
            _mockICacheUserService
                .Setup(c => c.GetCachedUser(_cacheUser.Id))
                .Returns(Task.FromResult(_cacheUser));

            var result = await _service.LimitTradeInHour(_cacheUser.Id);

            Assert.That(result, Is.True);

        }

        [Test]
        public async Task Valid_With_Exchanges_LimitTradeInHour_Test()
        {
            for (var i = 0; i < 5; i++)
            {
                _cacheUser.Exchanges.Add(new Exchange
                {
                    ExchangeDate = DateTime.Now.AddMinutes(-i),
                });
            }

            for (var i = 0; i < 5; i++)
            {
                _cacheUser.Exchanges.Add(new Exchange
                {
                    ExchangeDate = DateTime.Now.AddHours(-i),
                });
            }

            _mockICacheUserService
                .Setup(c => c.GetCachedUser(_cacheUser.Id))
                .Returns(Task.FromResult(_cacheUser));

            var result = await _service.LimitTradeInHour(_cacheUser.Id);

            Assert.That(result, Is.False);

        }

        [Test]
        public async Task Exactly_Than_Ten_Exchanges_LimitTradeInHour_Test()
        {

            for (var i = 0; i < 10; i++)
            {
                _cacheUser.Exchanges.Add(new Exchange
                {
                    ExchangeDate = DateTime.Now.AddMinutes(-i),
                });
            }

            _mockICacheUserService
                .Setup(c => c.GetCachedUser(_cacheUser.Id))
                .Returns(Task.FromResult(_cacheUser));

            var result = await _service.LimitTradeInHour(_cacheUser.Id);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task More_Than_Ten_Exchanges_LimitTradeInHour_Test()
        {

            for (var i = 0; i < 12; i++)
            {
                _cacheUser.Exchanges.Add(new Exchange
                {
                    ExchangeDate = DateTime.Now.AddMinutes(-i),
                });
            }

            _mockICacheUserService
                .Setup(c => c.GetCachedUser(_cacheUser.Id))
                .Returns(Task.FromResult(_cacheUser));

            var result = await _service.LimitTradeInHour(_cacheUser.Id);

            Assert.That(result, Is.False);
        }
    }
}
