using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using MeDirect.ExchangeApi.Entities;
using MeDirect.ExchangeApi.Infrastructure;
using MeDirect.ExchangeApi.Models;
using Microsoft.EntityFrameworkCore; 

namespace MeDirect.ExchangeApi.Services
{
    public interface IUserService
    {
        Task<CacheUser> Login(LoginModel loginModel);
        Task<bool> LimitTradeInHour(Guid userId);
        Task<bool> UpdateUserExchanges(Exchange exchange);
         
    }

    public class UserService : IUserService
    { 
        private readonly ICacheUserService _cacheUserService;
        private readonly MeDirectDbContext _context;

        public UserService(MeDirectDbContext context, ICacheUserService cacheUserService )
        { 
            _cacheUserService = cacheUserService;
            _context = context;
        }


        public async Task<CacheUser> Login(LoginModel loginModel)
        {
            if(loginModel.UserName == null || loginModel.Password == null)
                throw new Exception(Messages.UserNotFound);

            var user =await _context.Users
                .FirstOrDefaultAsync(c => c.UserName.Equals(loginModel.UserName.Trim())
                                          && c.Password == loginModel.Password.Trim());
            if (user == null)
                throw new Exception(Messages.UserNotFound);
          
            var cacheUser = new CacheUser
            {
                Id = user.Id,
                Name= user.Name,
                Family =user.Family,
                UserName=user.UserName,                
            };

            await _cacheUserService.AddUserToCache(cacheUser);
           
            return cacheUser;
        }

        public async Task<bool> LimitTradeInHour(Guid userId)
        {
          var user =  await _cacheUserService.GetCachedUser(userId);
          if (user == null)
              throw new Exception(Messages.IsNotLogin);

          var tradesInHour = user.Exchanges.Where(c=>DateTime.Now.Subtract(c.ExchangeDate).TotalHours < 1);

          var newUser = user with { Exchanges = new List<Exchange>(tradesInHour) };

          await _cacheUserService.UpdateUserToCache(newUser);
          
          return user.Exchanges.Count < 10;
        }

        public async Task<bool> UpdateUserExchanges(Exchange exchange)
        {
            var user = await _cacheUserService.GetCachedUser(exchange.UserId);
            if (user == null)
                throw new Exception(Messages.UserNotFound);

            user.Exchanges.Add(exchange);

          return  await _cacheUserService.UpdateUserToCache(user);
        }

        
    }
}
