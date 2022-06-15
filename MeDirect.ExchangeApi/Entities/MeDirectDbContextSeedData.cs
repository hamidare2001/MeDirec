using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace MeDirect.ExchangeApi.Entities
{
    public static class MeDirectDbContextSeedData
    {
        public static void SeedData(this IServiceScopeFactory scopeFactory)
        {
            using var serviceScope = scopeFactory.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<MeDirectDbContext>();
            if (context == null || context.Users.Any())
                return;

            var persons = new List<User>
            {
                new()
                {
                    Name = "Hamidreza",
                    Family = "Ardekani",
                    UserName = "Hamid84",
                    // it had better encrypt the password
                    Password = "12345678",
                },
                new()
                {
                    Name = "Sam",
                    Family = "Smith",
                    UserName = "Sam",
                    // it had better encrypt the password
                    Password = "23456789",
                },
                new()
                {
                    Name = "Robert",
                    Family = "DeNiro",
                    UserName = "RDeNiro",
                    // it had better encrypt the password
                    Password = "34567890",
                }
            };

            context.AddRange(persons);
            context.SaveChanges();
        }
    }
}
