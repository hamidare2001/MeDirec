using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using RateService.Models;

namespace MeDirect.ExchangeApi.Entities
{
    public class User
    {
        public Guid Id { get; set; } 
        public string Name { get; set; }        
        public string Family { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public virtual ICollection<Exchange> Exchanges { get; set; }= new List<Exchange>();
    }


    public class Exchange
    {
        public Guid Id { get; set; }

        [ForeignKey("user_id")]
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime ExchangeDate { get; set; }
        public Currency CurrencyFrom { get; set; }
        public Currency CurrencyTo { get; set; }
        public double Amount { get; set; }
    }
}
