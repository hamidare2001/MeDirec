using System.Threading.Tasks;
using MeDirect.ExchangeApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace MeDirect.Test.Helpers
{
    internal class FillDataHelper
    {
        public static async Task<MeDirectDbContext> FillDataBase()
        {
            var dbName = "MeDirect.Test";
            var dbContextOptions = new DbContextOptionsBuilder<MeDirectDbContext>()
                 .UseInMemoryDatabase(dbName)
                 .Options;

            var context = new MeDirectDbContext(dbContextOptions);


            var author = new User()
            {
                Name = "hamid",
                Family = "test",
                UserName = "test84",
                Password = "123456",
            };

            await context.Users.AddAsync(author);

            return await context.SaveChangesAsync() > 0
                ? context
                : null;
        }
    }
}
