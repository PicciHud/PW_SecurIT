//using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Devices.Client;
using System.IO.Ports;
using System.Text;


class Program
{
    static async Task Main()
    {
        //string connectionString = "Endpoint=sb://securit.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=RFtJ267H1qEUxXv73KqDILGAp23xMocRs+ASbCF4ajY=;";
        //string queueName = "securit_queue";
        string serialPortName = "COM7";
        int baudRate = 9600;

        // Configura la porta seriale
        using (SerialPort serialPort = new SerialPort(serialPortName, baudRate))
        {
            serialPort.DataReceived += SerialPort_DataReceived;
            serialPort.Open();

            Console.WriteLine("In attesa di dati dalla porta seriale. Premi CTRL+C per interrompere.");

            // Loop infinito per mantenere il worker in esecuzione
            while (true)
            {
                //await Task.Delay(1000);
            }
        }

        async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadLine();

            Console.WriteLine("data: " + data);

            if (data != "")
            {
                // Invia il messaggio al Service Bus
                await SendMessageToIoTHub(data);
            }
        }

    }



    static async Task SendMessageToIoTHub(string messageString)
    {
        // Connessione all'IoT Hub
        string iotHubConnectionString = "HostName=SecurIT.azure-devices.net;DeviceId=SecurIT-Device;SharedAccessKeyName=iothubowner;SharedAccessKey=2YH2GpB0s76SgyIcNnOeyCNI0fuvkPPoCggkZE+bkjs=";
        DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(iotHubConnectionString, TransportType.Mqtt);

        // Creazione del messaggio
        Message message = new Message(Encoding.UTF8.GetBytes(messageString));

        try
        {
            // Invio del messaggio all'IoT Hub
            await deviceClient.SendEventAsync(message);
            Console.WriteLine("Messaggio inviato con successo!");
            Console.WriteLine("Messaggio:" + messageString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore durante l'invio del messaggio: {ex.Message}");
        }

        // Chiusura della connessione
        await deviceClient.CloseAsync();
    }

    /*
    static async Task SendMessageToServiceBus(string connectionString, string queueName, string message)
    {
        await using var client = new ServiceBusClient(connectionString);
        var sender = client.CreateSender(queueName);
        var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(message));
        await sender.SendMessageAsync(serviceBusMessage);

        Console.WriteLine($"Messaggio inviato al Service Bus: {message}");
    }
    */
}






/*
using System;
using System.IO.Ports;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Azure.ServiceBus;

class Program
{
    static void Main(string[] args)
    {

        //Console.WriteLine("Premi Enter per inviare un messaggio al Service Bus o 'q' per uscire.");

        while (true)
        {

            
        }

        // Chiusura della connessione alla porta seriale e del client del Service Bus
    }

    static void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        // Lettura dei dati dalla porta seriale
        SerialPort serialPort = (SerialPort)sender;
        string data = serialPort.ReadLine();
        Console.WriteLine("Dati ricevuti dalla porta seriale: " + data);

        string serialPortName = "COM7";  // Nome della porta seriale da utilizzare
        string serviceBusConnectionString = "Endpoint=sb://securit.servicebus.windows.net/;SharedAccessKeyName=iothubroutes_ITS-QZER-ProjectWorkY2-Gruppo7;SharedAccessKey=MVpf/JfEAAjbUuAi66YyNApcCd0IiXYss+ASbNleajQ=;";  // Connection string del Service Bus
        string queueName = "securit_queue";  // Nome della coda del Service Bus

        // Creazione della connessione alla porta seriale
        SerialPort serialPort2 = new SerialPort(serialPortName, 9600);
        serialPort2.DataReceived += SerialPortDataReceived;
        serialPort2.Open();

        QueueClient queueClient = new QueueClient(serviceBusConnectionString, queueName);

        // Invio del messaggio al Service Bus
        string message = data;
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
        queueClient.SendAsync(new Message(messageBytes)).GetAwaiter().GetResult();
        Console.WriteLine("Messaggio inviato al Service Bus.");

        //queueClient.SendAsync(new Message(messageBytes)).GetAwaiter().GetResult();
        //Console.WriteLine("Messaggio inviato al Service Bus.");
    }
}
*/