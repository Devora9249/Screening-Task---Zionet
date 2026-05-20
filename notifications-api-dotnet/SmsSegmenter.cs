namespace NotificationApi;

public static class SmsSegmenter
{
    // SMS messages are limited to 160 characters per segment (GSM-7).
    public const int MaxSegmentChars = 160;

    // Returns the minimum number of SMS segments needed to deliver `message`
    // without splitting any word across segments. Used to report how many
    // billable SMS parts a notification will consume.
    public static int MinSegments(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return 0;
        var words = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0) return 0;
        return MinSegmentsFrom(words, 0);
    }

    private static int MinSegmentsFrom(string[] words, int start)
    {
        if (start >= words.Length) return 0;
        int best = int.MaxValue;
        int currentLen = 0;
        for (int end = start; end < words.Length; end++)
        {
            int add = currentLen == 0 ? words[end].Length : words[end].Length + 1;
            if (currentLen + add > MaxSegmentChars) break;
            currentLen += add;
            int rest = MinSegmentsFrom(words, end + 1);
            if (rest + 1 < best) best = rest + 1;
        }
        return best == int.MaxValue ? 0 : best;
    }
}
