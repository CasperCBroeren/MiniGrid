namespace MiniGrid.GameEngine;

public enum GameState
{
    NotStarted,
    Playing,
    Pauzed,
    Ended
}

public enum GameSpeed
{
    Normal = 208, // a game will take 30 minutes
    Fast = 104, // a game will take 15 minutes
    VeryFast = 52, // a game will take 7.5 minutes
    UltraFast = 26, // a game will take 3.75 minutes
    UnitTest = 1 // 9 seconds
}
public class Game
{
    public GameState GameState { get; set; } = GameState.NotStarted;
    private DateTime _lastUpdated = DateTime.UtcNow;

    public GameSpeed GameSpeed { get; set; }
    public DayInfo CurrentDay { get; private set; }

    public List<Player> Players { get; set; } = new List<Player>();

    public string GameCode { get; set; }

    public Game()
    {
        CurrentDay = new DayInfo(new GameTime(1, 1, 0));
        GameCode = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
    }

    public void Tick()
    {
        if (GameState == GameState.Playing)
        {
            var deltaTime = (DateTime.UtcNow - _lastUpdated).TotalMilliseconds;
            if (deltaTime > (int)GameSpeed)
            {
                _lastUpdated = DateTime.UtcNow;
                // Update asset yields for the current hour
                foreach(var player in Players)
                {
                    foreach (var asset in player.Assets)
                    {
                        // The asset should be active
                        if (asset.ActivatedOn != null && asset.ActivatedOn.AddHours(asset.RampRateInHours) < CurrentDay.Date)
                        {
                            // get hourly consumption
                            var consumption = ConsumerModel.CalculateConsumptionKw(player.Clients, CurrentDay);
                            var production = asset.ProductionPerHour(CurrentDay);
                            player.TotalProductionKWh += production;
                            player.TotalConsumptionKWh += consumption; 
                            player.TotalCarbonEmitted += asset.CarbonScore;
                        }
                    }
                }
                // Update to advance to a next hour                
                CurrentDay = CurrentDay.Progress();

                if (CurrentDay.Date.IsNewYear)
                {
                    GameState = GameState.Ended;
                    // Game ends, sort 

                }
            }           
        }
        else
        {
            _lastUpdated = DateTime.UtcNow;
        }
    }

    public void SetPlaying(params Player[] players)
    {
        Players.AddRange(players);
        GameState = GameState.Playing;
    }

    public async Task RunAsync(CancellationToken cancellationToken, Func<Game, Task> gamestate)
    {
        while (GameState == GameState.Playing 
            && !cancellationToken.IsCancellationRequested)
        {
            Tick();
            await gamestate(this);
        }
    }

    public void Run(CancellationToken cancellationToken)
    {
        while (GameState == GameState.Playing
            && !cancellationToken.IsCancellationRequested)
        {
            Tick(); 
        }
    }
}
