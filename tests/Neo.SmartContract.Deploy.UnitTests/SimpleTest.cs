using Xunit;

namespace Neo.SmartContract.Deploy.UnitTests;

public class SimpleTest
{
    [Fact]
    public void SimpleTest_ShouldPass()
    {
        // Simple test to verify test framework works
        Assert.True(true);
        Assert.Equal(2, 1 + 1);
    }
}
