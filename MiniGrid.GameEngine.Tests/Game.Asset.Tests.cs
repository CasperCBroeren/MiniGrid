using FluentAssertions;
using MiniGrid.GameEngine.Assets;

namespace MiniGrid.GameEngine.Tests;

public class GameTestsAssets
{
    [Fact]
    public void SolarVsWind()
    {
        // Arrange
        var game = new Game
        {
            GameSpeed = GameSpeed.UnitTest,            
        };
        var solarPlayer = new Player() {  CashInEuro = 1000000, TakesPartIn = game };
        solarPlayer.BuyAsset(
            new SolarPark()
            {
                   ActivatedOn = GameTime.StartOfGame            
            }
        );
        var windPlayer = new Player() { CashInEuro = 1000000, TakesPartIn = game };
        windPlayer.BuyAsset(
            new WindPark()
            {
                ActivatedOn = GameTime.StartOfGame
            }
        );
        game.SetPlaying(solarPlayer, windPlayer);

        // Act
        game.Run(TestContext.Current.CancellationToken);

        // Assert
        game.GameState.Should().Be(GameState.Ended);
        // In general a solar panel (in the neteherlands) will produce less energy than a wind turbine, so we expect the total production of the solar player to be less than that of the wind player
        solarPlayer.TotalProductionKWh.Should().BeLessThan(windPlayer.TotalProductionKWh);
        solarPlayer.TotalProductionKWh.Should().Be(14_700);  
        windPlayer.TotalProductionKWh.Should().BeInRange(45_000, 50_000);  
        // The total consumption should be the same for both players since they have the same number of clients
        solarPlayer.TotalConsumptionKWh.Should().Be(windPlayer.TotalConsumptionKWh);
    }    
}

