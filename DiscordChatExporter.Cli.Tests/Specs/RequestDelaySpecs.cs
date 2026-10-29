using System;
using System.Linq;
using DiscordChatExporter.Core.Discord;
using FluentAssertions;
using Xunit;

namespace DiscordChatExporter.Cli.Tests.Specs;

// Pure unit tests, no Discord token required
public class RequestDelaySpecs
{
    [Fact]
    public void I_can_rely_on_the_default_request_delay_to_not_delay_requests()
    {
        // Act
        var delay = RequestDelay.None;

        // Assert
        delay.IsEnabled.Should().BeFalse();
        delay.GetNextDelay(new Random(0)).Should().Be(TimeSpan.Zero);
        default(RequestDelay).IsEnabled.Should().BeFalse();
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(0, null)]
    [InlineData(0, 0)]
    [InlineData(-100, null)]
    [InlineData(-100, -50)]
    [InlineData(null, 0)]
    public void I_can_create_a_request_delay_that_is_disabled_when_the_maximum_is_zero(
        int? min,
        int? max
    )
    {
        // Act
        var delay = RequestDelay.Create(min, max);

        // Assert
        delay.IsEnabled.Should().BeFalse();
        delay.MinMilliseconds.Should().Be(0);
        delay.MaxMilliseconds.Should().Be(0);
        delay.GetNextDelay(new Random(0)).Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void I_can_create_a_request_delay_with_a_negative_minimum_which_is_treated_as_zero()
    {
        // Act
        var delay = RequestDelay.Create(-100, 50);

        // Assert
        delay.IsEnabled.Should().BeTrue();
        delay.MinMilliseconds.Should().Be(0);
        delay.MaxMilliseconds.Should().Be(50);
    }

    [Fact]
    public void I_can_create_a_request_delay_with_only_the_minimum_set_and_get_a_fixed_delay()
    {
        // Arrange
        var random = new Random(42);

        // Act
        var delay = RequestDelay.Create(300, null);

        // Assert
        delay.IsEnabled.Should().BeTrue();
        delay.MinMilliseconds.Should().Be(300);
        delay.MaxMilliseconds.Should().Be(300);
        Enumerable
            .Range(0, 100)
            .Select(_ => delay.GetNextDelay(random))
            .Should()
            .AllBeEquivalentTo(TimeSpan.FromMilliseconds(300));
    }

    [Fact]
    public void I_can_create_a_request_delay_with_a_maximum_below_the_minimum_which_is_raised_to_the_minimum()
    {
        // Arrange
        var random = new Random(42);

        // Act
        var delay = RequestDelay.Create(500, 100);

        // Assert
        delay.MinMilliseconds.Should().Be(500);
        delay.MaxMilliseconds.Should().Be(500);
        Enumerable
            .Range(0, 100)
            .Select(_ => delay.GetNextDelay(random))
            .Should()
            .AllBeEquivalentTo(TimeSpan.FromMilliseconds(500));
    }

    [Fact]
    public void I_can_get_request_delays_that_fall_within_the_configured_range()
    {
        // Arrange
        var random = new Random(42);
        var delay = RequestDelay.Create(400, 1800);

        // Act
        var samples = Enumerable
            .Range(0, 10_000)
            .Select(_ => delay.GetNextDelay(random).TotalMilliseconds)
            .ToArray();

        // Assert
        samples.Should().OnlyContain(ms => ms >= 400 && ms <= 1800);
        samples.Should().OnlyContain(ms => ms == Math.Floor(ms));
        samples.Distinct().Count().Should().BeGreaterThan(100);
    }

    [Fact]
    public void I_can_get_request_delays_that_include_both_bounds_of_the_range()
    {
        // Arrange
        var random = new Random(42);
        var delay = RequestDelay.Create(10, 11);

        // Act
        var samples = Enumerable
            .Range(0, 1_000)
            .Select(_ => delay.GetNextDelay(random).TotalMilliseconds)
            .Distinct()
            .Order()
            .ToArray();

        // Assert
        samples.Should().Equal(10, 11);
    }

    [Fact]
    public void I_can_create_a_request_delay_with_the_largest_possible_maximum_without_overflowing()
    {
        // Act
        var delay = RequestDelay.Create(int.MaxValue, null);

        // Assert
        delay.GetNextDelay(new Random(0)).Should().Be(TimeSpan.FromMilliseconds(int.MaxValue));
    }
}
