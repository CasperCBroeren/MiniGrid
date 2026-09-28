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
            GameSpeed = GameSpeed.UnitTest
        };
        var solarPlayer = new Player();
        solarPlayer.BuyAsset(
            new Solar()
            {
                   ActivatedOn = GameTime.StartOfGame            
            }
        );
        var windPlayer = new Player();
        windPlayer.BuyAsset(
            new Wind()
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
        solarPlayer.TotalProductionKWh.Should().Be(1_071_014); // TODO Isnt this more like 1.400.000 Kw
        windPlayer.TotalProductionKWh.Should().BeInRange(6_000_000, 6_500_000); // TODO Isnt this more like 6.000.000 Kwh
        // The total consumption should be the same for both players since they have the same number of clients
        solarPlayer.TotalConsumptionKWh.Should().Be(windPlayer.TotalConsumptionKWh);
    }    
}

