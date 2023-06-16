using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.ServiceBus;
using Microsoft.Extensions.Logging;

namespace MyFunctionApp
{
    public static class ServiceBusReader
    {
        [FunctionName("ServiceBusReader")]
        public static void Run([ServiceBusTrigger("securit_queue", Connection = "AzureWebJobsStorage")] string message,
            ILogger log)
        {
            log.LogInformation($"Received message: {message}");
        }
    }
}
