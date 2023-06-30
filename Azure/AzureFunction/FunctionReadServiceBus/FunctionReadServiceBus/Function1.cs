using FunctionReadServiceBus.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public static class Function1
{
    private static readonly HttpClient httpClient = new HttpClient();

    [FunctionName("Function1")]
    public static async Task Run(
        [ServiceBusTrigger("securit_queue", Connection = "AzureWebJobsStorage")] string message,
        ILogger log)
    {
        try
        {
            OpenDoorRequest openDoorRequest = new OpenDoorRequest();

            openDoorRequest.Id = 0;
            openDoorRequest.CodePic = message;
            openDoorRequest.CodeCloud = "";
            openDoorRequest.IdPic = 1;
            openDoorRequest.IdUser = 1;
            openDoorRequest.Name = "Filippo";
            openDoorRequest.SurName = "Saracco";
            openDoorRequest.IdHouse = 1;
            openDoorRequest.IdRoom = 1;
            openDoorRequest.Time = DateTime.Now;

            string messageBody = JsonSerializer.Serialize(openDoorRequest);

            // Effettua una richiesta POST API al tuo endpoint di inserimento nel database
            string databaseApiUrl = "https://localhost:7061/api/Access";
            var content = new StringContent(messageBody, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(databaseApiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                log.LogInformation("Dati inseriti nel database con successo.");
                //return new OkResult();
            }
            else
            {
                log.LogError($"{response}");
                //return new StatusCodeResult((int)response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Errore durante l'esecuzione della funzione.");
            //return new StatusCodeResult(500);
        }
    }
}



/*
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
*/