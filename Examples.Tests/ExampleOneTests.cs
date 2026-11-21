namespace Examples.Tests;

public class ExampleOneTests
{
    [Fact]
    public void TryGetValue_WhenTagIsMissing_ReturnsFalse()
    {
        var parser = new ExampleOne();
        var message = "8=FIX.4.4|9=145|35=D|"u8;

        var found = parser.TryGetValue(message, 99, out var value);

        Assert.False(found);
        Assert.True(value.IsEmpty);
    }

    [Fact]
    public void TryGetValue_WhenTagIsMissing_ReturnsTrue()
    {
        var parser = new ExampleOne();
        var message = "8=FIX.4.4|9=145|35=D|"u8;

        var found = parser.TryGetValue(message, 35, out var value);

        Assert.True(found);
        Assert.True(value.SequenceEqual("D"u8));
    }
}