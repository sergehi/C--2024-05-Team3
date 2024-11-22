// SignalR Hub
// Использование:

// Подключение:
//  const connection = new signalR.HubConnectionBuilder()
//               connection = new HubConnectionBuilder()
//            .WithUrl($"{url}/notifyhub") // Укажите URL gateway сервиса
//            .Build();
// // Подключаемся к Hub
// await connection.StartAsync();
// Console.WriteLine("Подключено к Hub");

// Входящие сообщения:
// using Common.SignalR;
// connection.On<SignalMessage>("MessageForClient", (message) =>
// {
//      Console.WriteLine($"Получено сообщение от {message.SenderModule}: {message.Body}");
// });

// Исходящие сообщения
// ...
// Сформировать message (см. Common.SignalR.SignalMessage)
// ...
// await connection.InvokeAsync("SendMessage", message);
//
using System.Collections.Concurrent;
using System.Threading;
using Common.SignalR;
using Microsoft.AspNetCore.SignalR;


namespace TaskTracker.Gateway.Controllers
{
    public class SignalHub : Hub
    {
        // Словарь активных(подключенных) пользователей
        private static readonly ConcurrentDictionary<string, UserConnection> _connections = new ConcurrentDictionary<string, UserConnection>();

        public SignalHub() { }

        public override Task OnConnectedAsync()
        {
            try
            {
                // Получаем идентификатор пользователя (например, из контекста)
                HttpContext? http_context = Context.GetHttpContext();

                string? strid = "";
                if (http_context is not null)
                {
                    HttpRequest? http_req = http_context.Request;
                    if (http_req is not null)
                    {
                        IHeaderDictionary headers = http_req.Headers;
                        if (headers is not null && headers.ContainsKey("userId"))
                            strid = headers["userId"];
                    }
                }
                if (strid is null)
                    throw new Exception("Invalid user");

                var guid = Guid.Parse(strid);
                if (guid != Guid.Empty)
                {
                    // Создаем новый объект UserConnection
                    var userConnection = new UserConnection
                    {
                        ConnectionId = Context.ConnectionId,
                        UserId = guid
                    };

                    // Добавляем соединение в словарь
                    _connections[strid] = userConnection;
                }
                return base.OnConnectedAsync();


            }
            catch (Exception)
            {
                throw;
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                // Удаляем соединение из словаря
                string? userId = Context.UserIdentifier; // Получаем идентификатор пользователя
                if (!string.IsNullOrEmpty(userId))
                {
                    _connections.TryRemove(userId, out _);
                }
                return base.OnDisconnectedAsync(exception);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Послать сообщение всем получателями из списка message.Receivers
        /// </summary>
        /// <param name="message"></param>
        /// <returns>Список пользователей, которым сообщение успешно послано(те, которые подключены в данный момент).</returns>
        public async Task<List<Guid>> SendMessage(SignalMessage message)
        {
            // Делаем копию для отправки что бы очистить список получателей - не надо их по сети гонять
            var messageToSend = message.Clone() as SignalMessage;
            messageToSend?.Recipients.Clear();

            var foundRecipients = new List<Guid>();
            // Проверяем, что список получателей не пуст
            if (message.Recipients != null && message.Recipients.Count > 0)
            {
                // Посылаем сообщения клиентам в параллель
                await Parallel.ForEachAsync(message.Recipients, async (recipient, cancellationToken) =>
                {
                    if (_connections.TryGetValue(recipient.ToString(), out var userConnection))
                    {
                        await Clients.Client(userConnection.ConnectionId).SendAsync("MessageForClient", messageToSend, cancellationToken);
                        lock (foundRecipients) // lock для безопасного добавления в список
                        {
                            foundRecipients.Add(recipient);
                        }
                    }
                });
            }
            return foundRecipients;
        }

        /// <summary>
        /// Послать всем подключенным
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendBroadcastMessage(SignalMessage message)
        {
            try
            {
                // Делаем копию для отправки что бы очистить список получателей - не надо их по сети гонять
                var messageToSend = message.Clone() as SignalMessage;
                messageToSend?.Recipients.Clear();
                await Clients.All.SendAsync("MessageForClient", messageToSend);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
