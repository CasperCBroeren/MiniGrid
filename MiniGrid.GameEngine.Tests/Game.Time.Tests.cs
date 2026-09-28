using FluentAssertions;

namespace MiniGrid.GameEngine.Tests;

public class GameTestsTime
{
    [Fact]
    public void GameStarts_UnitTestSpeed_AndEndsAfterOneGameYear()
    {
        // Arrange
        var game = new Game
        {
            GameSpeed = GameSpeed.UnitTest
        };
        game.SetPlaying(new Player());

        // Act
        game.Run(TestContext.Current.CancellationToken);

        // Assert
        game.GameState.Should().Be(GameState.Ended);
    }



    [Fact]
    public void GameStarts_UltraFastSpeed_AndEndsAfterOneGameYear()
    {
        // Arrange
        var game = new Game
        {
            GameSpeed = GameSpeed.UltraFast
        };
        game.SetPlaying(new Player());

        // Act
        game.Run(TestContext.Current.CancellationToken);

        // Assert
        game.GameState.Should().Be(GameState.Ended);
        game.CurrentDay.Date.IsNewYear.Should().BeTrue();
    }
}

