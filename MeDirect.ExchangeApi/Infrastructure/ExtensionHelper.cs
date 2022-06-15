
using Microsoft.Extensions.Logging;

namespace MeDirect.ExchangeApi.Infrastructure
{
    public static class ExtensionHelper
    {
        public static void Error(this ILogger logger, string method, object input, string message) 
        { 
            logger.LogError("Error in {@Action} input:{@input} with message {@message}", method , input, message);
        }
    }
}
