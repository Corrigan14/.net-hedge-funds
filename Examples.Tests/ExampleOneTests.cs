using Examples;

namespace Examples.Tests;

public class ExampleOneTests
{
    [Fact]
    public void TryGetValue_WhenTagIsMissing_ReturnsFalse()
    {
        var parser = new ExampleOne();
        ReadOnlySpan<byte> message = "8=FIX.4.4|9=145|35=D|"u8;

        bool found = parser.TryGetValue(message, 99, out var value);

        Assert.False(found);
        Assert.True(value.IsEmpty);
    }
}
