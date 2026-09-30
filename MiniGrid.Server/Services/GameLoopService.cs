using MiniGrid.Server.Services;

namespace MiniGrid.Server.Services;

public class GameLoopService : BackgroundService
{
    private readonly ILogger<GameLoopService> _logger;
    private readonly TimeSpan _tickInterval = TimeSpan.FromMilliseconds(100);

    public GameLoopService(ILogger<GameLoopService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Game loop service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                GameManager.TickAllGames();
                await Task.Delay(100);
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
