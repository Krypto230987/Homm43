using System.Net.Sockets;
using System.Text;

namespace ChatClient_;

internal class Client
{
    public async Task RunAsync(string host, int port)
    {
        using TcpClient client = new TcpClient();
        try
        {
            client.Connect(host, port);

            var networkStream = client.GetStream();
            var reader = new StreamReader(networkStream, Encoding.Unicode);
            var writer = new StreamWriter(networkStream, Encoding.Unicode);
            Console.Write("Введите свое имя: ");

            string userName = Console.ReadLine();

            Console.WriteLine($"{userName}, добро пожаловать в чат.");


            // Запускаем задачу получения данных

            var receiveTask = ReceiveMessagesAsync(reader);

            // Запускаем задачу чтения данных

            var sendTask = SendMessagesAsync(writer, userName);

            await Task.WhenAny(receiveTask, sendTask);
        }

        catch (Exception ex)

        {
            Console.WriteLine(ex);
        }


        client.Close();
    }


    async Task SendMessagesAsync(StreamWriter writer, string username)

    {
        await writer.WriteLineAsync(username);

        await writer.FlushAsync();


        Console.WriteLine("Для отправки сообщений введите сообщение и нажмите Enter");

        while (true)

        {
            string? message = Console.ReadLine();

            if (string.IsNullOrEmpty(message)) continue;


            await writer.WriteLineAsync(message);

            await writer.FlushAsync();
        }
    }


    async Task ReceiveMessagesAsync(StreamReader reader)

    {
        while (true)

        {
            try

            {
                // считываем ответ в виде строки

                string? message = await reader.ReadLineAsync();

                // если пустой ответ, ничего не выводим на консоль

                if (string.IsNullOrEmpty(message)) continue;

                Console.WriteLine(message);
            }

            catch (Exception ex)

            {
                Console.WriteLine(ex);

                break;
            }
        }
    }
}