using FluentAssertions;

namespace MiniGrid.GameEngine.Tests;

public class ConsumerModelTest
{
    [Fact]
    public void ConsumerModelOverAYear()
    {
        // Arrange
        var result = 0;
        var dayInfo = new DayInfo(GameTime.StartOfGame);
        // Act
        for (var i = 0; i < 365; i++)
        {
            result += ConsumerModel.CalculateConsumptionKw(1, dayInfo);
            dayInfo = dayInfo.Progress();
        }
        
        // Assert
       result.Should().Be(2545);
    }
}
