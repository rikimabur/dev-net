using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace SignalRServer.SignalR
{
    public class ChatHub : Hub
    {
        /// <summary>
        /// Stores the mapping of user identifiers to their connection IDs.
        /// </summary>
        private static ConcurrentDictionary<string, string> Connections = new();

        /// <summary>
        /// Handles new client connections.
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            // We get the userId directly by connection.GetHttpContext()?.Request.Query["userId"]
            // OR we create a new QueryStringUserIdProvider.cs that inherits IUserIdProvider
            var userId = Context.UserIdentifier;
            ArgumentNullException.ThrowIfNull(userId);
            if (Connections.ContainsKey(userId))
            {
                Connections.TryAdd(userId, Context.ConnectionId);
            }
            //userConnections.AddOrUpdate(userId, Context.ConnectionId, (_, _) => Context.ConnectionId);
            Console.WriteLine($"{userId} Client connected: {Context.ConnectionId}");

            await base.OnConnectedAsync();
        }
        /// <summary>
        /// Handles client disconnection.
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier; 
            var connectionId = Context.ConnectionId;

            // Remove the connectionId from the dictionary
            var userToRemove = Connections.FirstOrDefault(kvp => kvp.Value == connectionId).Key;

            if (!string.IsNullOrEmpty(userToRemove))
            {
                Connections.TryRemove(userToRemove, out _);
            }

            Console.WriteLine($"{userId} Client disconnected: {Context.ConnectionId}");

            await base.OnDisconnectedAsync(exception);
        }
        /// <summary>
        /// Sends a message to all connected clients.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string user, string message)
        {
            Console.WriteLine($"Message received from {user}: {message}");
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        /// <summary>
        /// Sends a message to a specific user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageToUser(string user, string message)
        {
            var userId = Context.UserIdentifier;

            if (Connections.TryGetValue(user, out var connId))
            {
                Console.WriteLine($"{user} Message received from {userId}: {message}");
                await Clients.Client(connId).SendAsync("ReceiveMessage", userId, message);
            }
        }
    }
}
