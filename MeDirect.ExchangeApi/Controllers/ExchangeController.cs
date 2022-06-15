using System;
using System.Net;
using System.Threading.Tasks;
using MeDirect.ExchangeApi.Infrastructure;
using MeDirect.ExchangeApi.Models;
using MeDirect.ExchangeApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MeDirect.ExchangeApi.Controllers
{
    [ApiController]
    [Route("[action]")]
    public class ExchangeController : ControllerBase
    {
        private readonly ILogger<ExchangeController> _logger;
        private readonly IExchangeService _exchangeService;
        private readonly IUserService _userService;

        public ExchangeController(ILogger<ExchangeController> logger, IExchangeService exchangeService, IUserService userService)
        {
            _logger = logger;
            _exchangeService = exchangeService;
            _userService = userService;
        }



        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            try
            {
                var user = await _userService.Login(loginModel);
                _logger.LogInformation("user {@User} was login", user.Id);
                return Ok(user);
            }
            catch (Exception e)
            {
                _logger.Error("Login", loginModel, e.Message);
                return StatusCode(500, e.Message);
            }
        }
        

        [HttpPost]
        public async Task<IActionResult> GetLastRate(GetRateModel getRateModel)
        {
            try
            {
                var rate = await _exchangeService.GetLastRate(getRateModel);
                return Ok(rate);
            }
            catch (Exception e)
            {
                _logger.Error( "GetLastRate",getRateModel,e.Message);
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Exchange(Trade trade)
        {
            try
            {
                var exchange = await _exchangeService.ExchangeDone(trade);
                if (exchange)
                    return Ok();

                return StatusCode(500, Messages.ServerError);

            }
            catch (Exception e)
            {
                _logger.Error("Exchange", trade, e.Message);
                return StatusCode(500, e.Message);
            }
            

        }
    }
}
