using XTI_App.Abstractions;

namespace XTI_App.Api;

public sealed class ActionThrottledLogPathBuilder<TActionBuilder> where TActionBuilder : IAppApiActionBuilder
{
    private readonly ActionThrottledLogIntervalBuilder<TActionBuilder> requestIntervalBuilder;
    private readonly ActionThrottledLogIntervalBuilder<TActionBuilder> exceptionIntervalBuilder;

    public ActionThrottledLogPathBuilder(TActionBuilder actionBuilder)
    {
        requestIntervalBuilder = new(actionBuilder);
        exceptionIntervalBuilder = new(actionBuilder);
    }

    public ActionThrottledLogIntervalBuilder<TActionBuilder> RequestIntervalBuilder { get => requestIntervalBuilder; }

    public ActionThrottledLogIntervalBuilder<TActionBuilder> ExceptionIntervalBuilder { get => exceptionIntervalBuilder; }

    public ThrottledLogXtiPath Build(XtiPath actionPath) =>
        new
        (
            xtiPath: actionPath,
            requestInterval: requestIntervalBuilder.Interval,
            exceptionInterval: exceptionIntervalBuilder.Interval
        );
}

public sealed class ActionThrottledLogIntervalBuilder<TActionBuilder> where TActionBuilder : IAppApiActionBuilder
{
    private readonly TActionBuilder actionBuilder;

    public ActionThrottledLogIntervalBuilder(TActionBuilder actionBuilder)
    {
        this.actionBuilder = actionBuilder;
    }

    internal TimeSpan Interval { get; private set; }

    public TActionBuilder For(TimeSpan ts)
    {
        Interval = ts;
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
