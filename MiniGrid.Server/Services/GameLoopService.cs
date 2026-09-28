using MiniGrid.Server.Services;

namespace MiniGrid.Server.Services;

public class GameLoopService : BackgroundService
{
    private readonly GameManager _gameManager;
    private readonly ILogger<GameLoopService> _logger;
    private readonly TimeSpan _tickInterval = TimeSpan.FromMilliseconds(100);

    public GameLoopService(GameManager gameManager, ILogger<GameLoopService> logger)
    {
        _gameManager = gameManager;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Game loop service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _gameManager.TickAllGames();
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
