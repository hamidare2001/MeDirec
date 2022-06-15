using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeDirect.ExchangeApi;
using MeDirect.ExchangeApi.Entities;
using MeDirect.ExchangeApi.Infrastructure;
using MeDirect.ExchangeApi.Models;
using MeDirect.ExchangeApi.Services;
using MeDirect.Test.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using NUnit.Framework;
using RateService.Models;

namespace MeDirect.Test
{
    public class RedisTest
    {
        private DependencyResolverHelper _serviceProvider;
        private IDistributedCache _cache;
        private ICacheProvider _cacheProvider;
        private ICacheUserService _cacheUserService;

        [SetUp]
        public async Task Setup()
        {
            var webHost = WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .Build();
            _serviceProvider = new DependencyResolverHelper(webHost);

            
            _cacheUserService =  _serviceProvider.GetService<ICacheUserService>();
        }

        [Test]
        public async Task TestRedis()
        {
            var id = Guid.NewGuid();
            await _cacheUserService.AddUserToCache(new CacheUser
            {
                Id = id,
                UserName = "a",
                Name = "s",
                Family = "sd",
                Exchanges = new List<Exchange>
                {
                    new Exchange()
                    {
                        Id = Guid.NewGuid(),
                        CurrencyFrom = Currency.AED,
                        CurrencyTo = Currency.ALL,
                        Amount = 0,
                        ExchangeDate = DateTime.Now,
                    }
                }

            });

            var t =await _cacheUserService.GetCachedUser(id);

            Assert.That(t.Id,Is.EqualTo(id));
        }
    }
}
