using Vernou.Ariane.Tools.Tests.Core;

namespace Vernou.Ariane.Tools.Tests.Commands;

public sealed class GraphCommandTest
{
    [Fact]
    public async Task RunAsync()
    {
        // Arrange

        var output = new FakeOutput();
        var command = new Tools.Commands.GraphCommand("""AssetsFiles\ConsoleWithoutDependency.assets.json""");

        // Act

        await command.RunAsync();

        // Assert
    }
}
