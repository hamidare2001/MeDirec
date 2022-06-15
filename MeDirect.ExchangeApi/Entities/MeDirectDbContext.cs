using System;
using Microsoft.EntityFrameworkCore; 

namespace MeDirect.ExchangeApi.Entities
{
    public class MeDirectDbContext : DbContext
    {

        public DbSet<User> Users { get; set; }
        public DbSet<Exchange> Exchanges { get; set; }

        public MeDirectDbContext(DbContextOptions<MeDirectDbContext> options)
            : base(options)
        {
        }
    }
}
