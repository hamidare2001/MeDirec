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
    public class Tests
    {
        
        private UserService _service;
        private Mock<ICacheUserService> _mockICacheUserService; 

        [SetUp]
        public async Task Setup()
        { 
            _mockICacheUserService = new Mock<ICacheUserService>();
            var context = await FillDataHelper.FillDataBase();
            _service = new UserService(context, _mockICacheUserService.Object);
        }



        [TestCase("test84", "123456")]
        [TestCase(" test84", "123456 ")]
        [TestCase("test84 ", " 123456 ")]
        [TestCase("test84", "123456 ")]
        public async Task Login_Success_Test(string userName, string password)
        {

            // Act
            var cacheUser = await _service.Login(new LoginModel
            {
                UserName = userName,
                Password = password
            });

            // Assert
            Assert.That(cacheUser.Name, Is.EqualTo("hamid"));
            Assert.That(cacheUser.Family, Is.EqualTo("test"));
            Assert.That(cacheUser.UserName, Is.EqualTo("test84"));
        }


        [TestCase("hamid","")]
        [TestCase("", "123456")]
        [TestCase("  ","  ")]
        [TestCase(null, "123456")]
        [TestCase("test84", null)]
        [TestCase(null, null)] 
        [TestCase("Test84", "123456")] 
        [TestCase("test84", "123 456")] 
        public void Login_Fail_Test(string userName, string password)
        {
            var ex = Assert.ThrowsAsync<Exception>( async () =>
            {
               await _service.Login(new LoginModel { UserName = userName, Password = password });
            
            });

            if (ex != null) 
                Assert.That(ex.Message, Is.EqualTo(Messages.UserNotFound));
        }
         

        
    }
}