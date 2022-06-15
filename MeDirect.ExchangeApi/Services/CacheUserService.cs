using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MeDirect.ExchangeApi.Entities;
using MeDirect.ExchangeApi.Infrastructure;
using MeDirect.ExchangeApi.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace MeDirect.ExchangeApi.Services
{
    public interface ICacheUserService
    {
        Task<CacheUser> GetCachedUser(Guid userId);
        Task<bool> AddUserToCache(CacheUser user);
        Task Logout(Guid userId);
        Task<bool> UpdateUserToCache(CacheUser user);
    }

    
    public class CacheUserService : ICacheUserService
    {
        private readonly ICacheProvider _cacheProvider;
        
        private double CacheTimeToLive = 1;

        public CacheUserService(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }
        



        public async Task<CacheUser> GetCachedUser(Guid userId)
        {
            return await _cacheProvider.GetFromCache<CacheUser>(userId.ToString());
        }
 
        public async Task<bool> AddUserToCache(CacheUser user)
        {
            var cacheEntryOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(CacheTimeToLive));

            await _cacheProvider.SetCache(user.Id.ToString(), user, cacheEntryOptions);

            return true;
        }
        
        public async Task Logout(Guid userId)
        {
            await _cacheProvider.ClearCache(userId.ToString());
        }

        public async Task<bool> UpdateUserToCache(CacheUser user)
        {
            await Logout(user.Id);
           return await AddUserToCache(user);
        }
         
    }
}
