using Microsoft.AspNetCore.SignalR;
using MiniGrid.GameEngine;
using MiniGrid.Server.Hubs;
using MiniGrid.Server.Services;

namespace MiniGrid.Server.Services;

public class GameLoopService : BackgroundService
{
    private readonly ILogger<GameLoopService> _logger;
    private readonly IGameHub gameHub;
    private readonly TimeSpan _tickInterval = TimeSpan.FromMilliseconds((int)GameSpeed.UltraFast);

    public GameLoopService(ILogger<GameLoopService> logger, IGameHub gameHub)
    {
        _logger = logger;
        this.gameHub = gameHub;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Game loop service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                GameManager.TickAllGames(gameHub);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in game loop");
            }

            await Task.Delay(_tickInterval, stoppingToken);
        }

        _logger.LogInformation("Game loop service stopped");
    }
}
