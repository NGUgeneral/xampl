using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using xampl.Services.ConfigOptionsService;

namespace xampl.Hubs
{
    public class ConsoleHub(
        ILogger<ConsoleHub> logger,
        IOptions<ConfigOptions> configOptions
    ) : Hub
    {
        private readonly ILogger<ConsoleHub> _logger = logger;
        private readonly ConfigOptions _config = configOptions.Value;

        public async Task SendCommand(string command)
        {
            await Clients.Caller.SendAsync("ReceiveOutput", GetCommandResponse(command));
        }

        private string GetCommandResponse(string command)
        {   
            //TODO: implement command parsing logic,
            //i.g. mapping commands and generic response for unavailable commands;
            return command.Trim().ToLower() switch
            {
                "--help" =>
                    "test               - test connection to server",
                "test" => 
                    "This message was generated on the server. SignalR communicating correctly.",
                "rl_url" => _config.RateLimiterUrl,
                _ =>
                    $"Command unknown: {command}"
            };
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("Client connected: {connectionId}", Context.ConnectionId);
            await Clients.Caller.SendAsync("ReceiveOutput", "Connected to the server. Type a command!");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogError("Client disconnected: {connectionId} (Reason: {exMessage})", Context.ConnectionId, exception?.Message);
            await base.OnDisconnectedAsync(exception);
        }
    }
}