using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using MeDirect.ExchangeApi.Infrastructure;
using MeDirect.ExchangeApi.Models;
using Microsoft.Extensions.Logging;
using RateService.Models;
using RestSharp;

namespace MeDirect.ExchangeApi.Services
{
    public interface IRateApi
    { 
        Task<RateCurrency> GetRate(Currency currency, List<Currency> symbols, double? amount = null, DateTime? date = null);
    }
    public class RateApi: IRateApi
    {
        private readonly ILogger<RateApi> _logger;

        // it is better move in into the app.setting and encrypt them
        string apikey = "wOtTXhiv4k4FFN90KJovrT1agI5RItFB";
        string providerUrl = "https://api.apilayer.com/fixer";

        public RateApi(ILogger<RateApi> logger)
        {
            _logger = logger;
        }

        public async Task<RateCurrency> GetRate(Currency currency, List<Currency> symbols, double? amount = null, DateTime? date = null)
        {
            try
            {
                var symbolsString = "&symbols=" + string.Join(",", symbols);
                var amountString = amount.HasValue ? "&amount=" + amount.Value : string.Empty;
                var dateString = date.HasValue ? "&date=" + date.Value.ToString("YYYY-MM-DD") : string.Empty;

                var baseUrl = providerUrl + $"/latest?base={currency}{symbolsString}{amountString}{dateString}";
                var client = new RestClient(baseUrl);


                var request = new RestRequest();
                request.AddHeader("apikey", apikey);

                var response = await client.ExecuteAsync(request);


                //sample of response
                //var result = "{" +
                //             "   \"success\": true," +
                //             "    \"timestamp\": 1654844054," +
                //             "    \"base\": \"GBP\"," +
                //             "    \"date\": \"2022-06-10\", " +
                //             "    \"rates\": { " +
                //             "          \"EUR\": 0.940398, " +
                //             "          \"GBP\": 0.800689 " +
                //             "      }  " +
                //             "    } ";


                if (response.Content != null)
                {
                    var rate = JsonSerializer.Deserialize<RateCurrency>(response.Content);
                    if (rate != null)
                    {
                        rate.CurrencyBase = currency;
                        return rate;
                    }
                }

                _logger.Error("GetRate", currency, "response.Content is null");
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error("GetRate", currency,ex.Message);
                return null;
            }
 
        }
    }
}
