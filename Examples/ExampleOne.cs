using System.Buffers.Text;

namespace Examples;

public class ExampleOne
{
    public bool TryGetValue(ReadOnlySpan<byte> fixMessage, int tag, out ReadOnlySpan<byte> value)
    {
        ReadOnlySpan<byte> remaining = fixMessage;
        value = default;
        while (!remaining.IsEmpty)
        {
            var pipeIndex = remaining.IndexOf((byte)'|');

            if (pipeIndex == -1) break;

            var field = remaining.Slice(0, pipeIndex);

            var startIndex = field.IndexOf((byte)'=');

            if (startIndex == -1) break;

            var tagBytes = field.Slice(0, startIndex);

            bool parsed = Utf8Parser.TryParse(tagBytes, out int parsedTag, out int bytesConsumed);
            bool matches = parsed && bytesConsumed == tagBytes.Length && parsedTag == tag;
            if (matches)
            {
                value = field.Slice(startIndex + 1, pipeIndex);

                return true;
            }

            remaining = remaining.Slice(pipeIndex + 1);
        }

        return false;
    }
}