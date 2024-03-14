namespace ChatServer;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class Clients
{
protected internal string Id { get; } = Guid.NewGuid().ToString();
   protected internal StreamWriter Writer { get; }
   protected internal StreamReader Reader { get; }
   private TcpClient _client;
   private Server _server; // объект сервера
   public Clients(TcpClient tcpClient, Server serverObject)
   {
       _client = tcpClient;
       _server = serverObject;
       // получаем NetworkStream для взаимодействия с сервером
       var stream = _client.GetStream();
       // создаем StreamReader для чтения данных
       Reader = new StreamReader(stream, Encoding.Unicode);
       // создаем StreamWriter для отправки данных
       Writer = new StreamWriter(stream, Encoding.Unicode);
       // Добавляем клиента на сервер
       _server.AddClient(this);
   }
   public async Task ProcessAsync()
   {
       try
       {
           // получаем имя пользователя
           string? userName = await Reader.ReadLineAsync();
           string? message = $"{userName} вошел в чат";
           // посылаем сообщение о входе в чат всем подключенным пользователям
           await _server.BroadcastMessageAsync(message, Id);
           Console.WriteLine(message);
           // в бесконечном цикле получаем сообщения от клиента
           try
           {
               while (true)
               {
                   message = await Reader.ReadLineAsync();
                   if (message == null) continue;
                   message = $"{userName}: {message}";
                   Console.WriteLine(message);
                   await _server.BroadcastMessageAsync(message, Id);
               }
           }
           catch
           {
               message = $"{userName} покинул чат";
               Console.WriteLine(message);
               await _server.BroadcastMessageAsync(message, Id);
           }
       }
       catch (Exception e)
       {
           Console.WriteLine(e.Message);
       }
       finally
       {
           // в случае выхода из цикла закрываем ресурсы
           _server.RemoveClient(Id);
       }
   }
   // закрытие подключения
   protected internal void Close()
   {
       Writer.Close();
       Reader.Close();
       _client.Close();
   }
}