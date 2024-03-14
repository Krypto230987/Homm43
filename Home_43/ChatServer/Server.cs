using System.Net;
using System.Net.Sockets;

namespace ChatServer;

public class Server
{
    private TcpListener _tcpListener  = new TcpListener(IPAddress.Any, 11000); // сервер для прослушивания
    private Dictionary<string, Clients> _clients = new Dictionary<string, Clients>(); // все подключения
    public List<string> loggedinUsers = new List<string>();
    protected internal async Task BroadcastMessageAsync(string message, string id)
    {
        foreach (var (_, client) in _clients)
        {
            if (client.Id != id) // если id клиента не равно id отправителя
            {
                while (true)
                {
                    await client.Writer.WriteLineAsync(message); //передача данных
                    await client.Writer.FlushAsync();
                    LoginUser(message);
                }
                
            }
        }
    }
    private bool IsUserLoggedIn(string username)
    {
        return loggedinUsers.Contains(username);
    }
    // Метод для логина пользователя
    public void LoginUser(string username)
    {
        if(loggedinUsers.Contains(username)) 
        {
            Console.WriteLine($"Пользователь {username} уже залогинен. Пожалуйста, введите другое имя:");
            // здесь можно запросить у клиента новое имя и вызвать рекурсивно LoginUser
        }
        else
        {
            loggedinUsers.Add(username);
            Console.WriteLine($"Пользователь {username} успешно залогинен.");
        }
    }
    protected internal void AddClient(Clients client)
    {
        _clients.Add(client.Id, client);
    }
    protected internal void RemoveClient(string id)
    {
        if (!_clients.Remove(id, out var client))
            return;
        client.Close();
    }
    protected internal async Task Start()
    {
        try
        {
            _tcpListener.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");
            while (true)
            {
                TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync();
                Clients clientObject = new Clients(tcpClient, this);
                Task.Run(clientObject.ProcessAsync);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            DisconnectAll();
        }
    }
    protected internal void DisconnectAll()
    {
        foreach (var (_, client) in _clients)
        {
            client.Close(); //отключение клиента
        }
        _tcpListener.Stop(); //остановка сервера
    }
}
