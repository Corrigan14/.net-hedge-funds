using System.Text;

namespace Examples.Tests;

public class ExampleOneTests
{
    [Theory]
    [InlineData("8=FIX.4.4|9=145|35=D|", 99, false, "")]
    [InlineData("8=FIX.4.4|9=145|35=D|", 35, true, "D")]
    [InlineData("8=FIX.4.4|9=145|35=D|", 8, true, "FIX.4.4")]
    [InlineData("35=D|", 35, true, "D")]
    [InlineData("35=|", 35, true, "")]
    public void TryGetValue_ReturnsExpectedResult(
        string message,
        int tag,
        bool expectedFound,
        string expectedValue
    )
    {
        var parser = new ExampleOne();
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var expectedBytes = Encoding.UTF8.GetBytes(expectedValue);

        var found = parser.TryGetValue(messageBytes, tag, out var value);
        Assert.Equal(expectedFound, found);
        Assert.True(value.SequenceEqual(expectedBytes));
    }

    [Theory]
    [InlineData("35D|")] // Missing equals sign.
    [InlineData("abc=D|")] // Non-numeric tag.
    [InlineData("35x=D|")] // Only the tag's prefix is numeric.
    [InlineData("=D|")] // Missing tag.
    [InlineData("35=D")] // Missing closing delimiter.
    [InlineData("|")] // Empty field.
    public void TryGetValue_WhenFieldIsMalformed_ReturnsFalseAndEmptyValue(
        string message
    )
    {
        var parser = new ExampleOne();
        var messageBytes = Encoding.UTF8.GetBytes(message);

        var found = parser.TryGetValue(messageBytes, 35, out var value);

        Assert.False(found);
        Assert.True(value.IsEmpty);
    }

    [Fact]
    public void TryGetValue_WhenCalledRepeatedly_AllocatesNoManagedBytes()
    {
        var message = "8=FIX.4.4|9=145|35=D|";
        var parser = new ExampleOne();
        var bytes = Encoding.UTF8.GetBytes(message);
        parser.TryGetValue(bytes, 35, out var value);
        var before = GC.GetAllocatedBytesForCurrentThread();

        for (var i = 0; i < 10000; i++) parser.TryGetValue(bytes, 35, out value);
        var after = GC.GetAllocatedBytesForCurrentThread();
        Assert.Equal(0L, after - before);
    }
}