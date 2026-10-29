using System;

namespace DiscordChatExporter.Core.Discord;

// Random delay inserted before every request sent to Discord, used to throttle requests
// below the rate limit. The default value represents no delay.
public readonly partial struct RequestDelay
{
    public int MinMilliseconds { get; }

    public int MaxMilliseconds { get; }

    public bool IsEnabled => MaxMilliseconds > 0;

    private RequestDelay(int minMilliseconds, int maxMilliseconds)
    {
        MinMilliseconds = minMilliseconds;
        MaxMilliseconds = maxMilliseconds;
    }

    public TimeSpan GetNextDelay(Random random) =>
        IsEnabled
            ? TimeSpan.FromMilliseconds(
                random.NextInt64(MinMilliseconds, (long)MaxMilliseconds + 1)
            )
            : TimeSpan.Zero;
}

public readonly partial struct RequestDelay
{
    public static RequestDelay None => default;

    // Negative minimum is treated as zero, missing maximum defaults to the minimum,
    // and a maximum below the minimum is raised to the minimum.
    public static RequestDelay Create(int? minMilliseconds, int? maxMilliseconds)
    {
        var min = Math.Max(0, minMilliseconds ?? 0);
        var max = Math.Max(min, maxMilliseconds ?? min);

        return new RequestDelay(min, max);
    }
}
