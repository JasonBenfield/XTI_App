using XTI_TempLog;

namespace XTI_App.Api;

public sealed class ActionThrottledLogIntervalBuilder<TActionBuilder> where TActionBuilder : IAppApiActionBuilder
{
    private readonly TActionBuilder actionBuilder;
    private readonly ThrottledLogIntervalBuilder intervalBuilder;

    public ActionThrottledLogIntervalBuilder(TActionBuilder actionBuilder, ThrottledLogIntervalBuilder intervalBuilder)
    {
        this.actionBuilder = actionBuilder;
        this.intervalBuilder = intervalBuilder;
    }

    public TActionBuilder For(TimeSpan ts)
    {
        intervalBuilder.For(ts);
        return actionBuilder;
    }

    public ActionNextThrottledLogIntervalBuilder<TActionBuilder> For(double quantity) => NextBuilder(quantity);

    public TActionBuilder ForOneMillisecond() => NextBuilder(1).Milliseconds();

    public TActionBuilder ForOneSecond() => NextBuilder(1).Seconds();

    public TActionBuilder ForOneMinute() => NextBuilder(1).Minutes();

    public TActionBuilder ForOneHour() => NextBuilder(1).Hours();

    private ActionNextThrottledLogIntervalBuilder<TActionBuilder> NextBuilder(double quantity) =>
        new(this, quantity);
}

public sealed class ActionNextThrottledLogIntervalBuilder<TActionBuilder> where TActionBuilder : IAppApiActionBuilder
{
    private readonly ActionThrottledLogIntervalBuilder<TActionBuilder> builder;
    private readonly double quantity;

    internal ActionNextThrottledLogIntervalBuilder(ActionThrottledLogIntervalBuilder<TActionBuilder> builder, double quantity)
    {
        this.builder = builder;
        this.quantity = quantity;
    }

    public TActionBuilder Milliseconds() => SetTimeSpan(TimeSpan.FromMilliseconds(quantity));

    public TActionBuilder Seconds() => SetTimeSpan(TimeSpan.FromSeconds(quantity));

    public TActionBuilder Minutes() => SetTimeSpan(TimeSpan.FromMinutes(quantity));

    public TActionBuilder Hours() => SetTimeSpan(TimeSpan.FromHours(quantity));

    private TActionBuilder SetTimeSpan(TimeSpan ts) => builder.For(ts);
}
