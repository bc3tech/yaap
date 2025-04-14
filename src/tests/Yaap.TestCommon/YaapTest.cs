namespace Yaap.TestCommon;
using System;
using System.Diagnostics.Metrics;

using Moq;
using Moq.AutoMock;

using Xunit.Abstractions;

public abstract class YaapTest
{
    protected ITestOutputHelper TestOutputHelper { get; }
    public Mock<TimeProvider> TimeMock { get; }
    protected AutoMocker Mocker { get; } = new();

    public YaapTest(ITestOutputHelper testOutputHelper)
    {
        this.TestOutputHelper = testOutputHelper;

        this.TimeMock = new Mock<TimeProvider>();
        this.TimeMock.Setup(i => i.GetUtcNow())
            .Returns(TimeProvider.System.GetUtcNow);
        this.TimeMock.SetupGet(i => i.LocalTimeZone)
            .Returns(TimeProvider.System.LocalTimeZone);
        this.TimeMock.SetupGet(i => i.TimestampFrequency)
            .Returns(() => TimeProvider.System.TimestampFrequency);
        this.TimeMock.Setup(i => i.GetTimestamp())
            .Returns(TimeProvider.System.GetTimestamp);
        this.TimeMock.Setup(i => i.CreateTimer(It.IsAny<TimerCallback>(), It.IsAny<object?>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>()))
            .Returns((TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period) =>
                TimeProvider.System.CreateTimer(callback, state, dueTime, period));

        this.Mocker.Use(this.TimeMock);
        this.Mocker.Use(new Meter("UnitTestMeter"));

        this.Mocker.Use<IServiceProvider>(this.Mocker);
    }
}
