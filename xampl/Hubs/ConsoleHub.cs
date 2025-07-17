using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace xampl.Hubs
{
    public class ConsoleHub : Hub
    {
        // This method will be called by the client (your browser's JavaScript)
        // The 'command' parameter will contain the text typed by the user in Xterm.js
        public async Task SendCommand(string command)
        {
            //TODO: implement command parsing logic, i.g. mapping commands and generic response for unavailable commands;

            // For now, we'll just echo the command and respond with "Hello World!"
            // In later steps, this is where you'd execute the .NET Console App command.

            Console.WriteLine($"Received command from client ({Context.ConnectionId}): {command}"); // Log on server side

            if (command.Trim().ToLower() == "run-c --help")
            {
                // Send a specific response back to the client that sent the command
                await Clients.Caller.SendAsync("ReceiveOutput", "Hello World!");
            }
            else
            {
                // Send a generic response back to the client
                await Clients.Caller.SendAsync("ReceiveOutput", $"Server received: {command}");
            }

            // You could also send to all clients:
            // await Clients.All.SendAsync("ReceiveOutput", $"A client typed: {command}");
        }

        // You can override OnConnectedAsync and OnDisconnectedAsync for connection events
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await Clients.Caller.SendAsync("ReceiveOutput", "Connected to the server. Type a command!");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId} (Reason: {exception?.Message})");
            await base.OnDisconnectedAsync(exception);
        }
    }
}