using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.IO.Ports;
using System.Text;
using ReadFromIoTHub.Models;

class Program
{
    private static string connectionString = "HostName=SecurIT.azure-devices.net;DeviceId=SecurIT-Device;SharedAccessKey=ISqkhHnKivwxni+q3pZc2cmuY9l2U4RQwEnIVhig1fI=";
    private static string serialPortName = "COM7";
    private static int baudRate = 115200;

    static async Task Main(string[] args)
    {
        // Inizializza la connessione all'IoT Hub
        DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);

        // Configura la porta seriale
        SerialPort serialPort = new SerialPort(serialPortName, baudRate);

        try
        {
            // Apri la porta seriale
            serialPort.Open();

            // Leggi i messaggi dall'IoT Hub in un ciclo infinito
            while (true)
            {
                Message receivedMessage = await deviceClient.ReceiveAsync();

                if (receivedMessage != null)
                {
                    try
                    {
                        // Leggi il payload del messaggio
                        string messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());

                        // fai il parsing del json in una istanza di "Data"
                        OpenDoorRequest data = JsonConvert.DeserializeObject<OpenDoorRequest>(messageData);

                        Console.WriteLine("Messaggio ricevuto nell'IoT Hub: " + messageData);

                        // prepara il pacchetto di dati del protocollo da inviare al pic
                        // quindi id del destinatario, messaggio e durata del messaggio

                        string protocolData = data.CodeCloud;
                        //string protocolData = string.Join("_", data.PicId, data.Message, data.IntervalTime);

                        // Scrivi il messaggio sulla porta seriale
                        serialPort.WriteLine(protocolData);

                        Console.WriteLine("Messaggio inviato alla porta seriale: " + protocolData);

                        // Completa l'elaborazione del messaggio
                        await deviceClient.CompleteAsync(receivedMessage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Errore nell'invio del messaggio sulla porta seriale: " + ex.Message);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante la comunicazione con la porta seriale: " + ex.Message);
        }
        finally
        {
            // Chiudi la porta seriale alla fine
            serialPort.Close();
        }
    }

    static async Task ReceiveCloudToDeviceMessage(DeviceClient deviceClient)
    {
        while (true)
        {
            // Ricevi il messaggio dal cloud
            Message receivedMessage = await deviceClient.ReceiveAsync();

            if (receivedMessage != null)
            {
                try
                {
                    // Leggi il payload del messaggio
                    string messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                    Console.WriteLine("Messaggio ricevuto dal cloud: " + messageData);

                    // Completa l'elaborazione del messaggio per rimuoverlo dalla coda
                    await deviceClient.CompleteAsync(receivedMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Errore nell'elaborazione del messaggio: " + ex.Message);
                }
            }
        }
    }
}
