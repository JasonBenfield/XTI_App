using Microsoft.Extensions.DependencyInjection;
using XTI_App.Abstractions;
using XTI_App.Api;

namespace XTI_ConsoleApp.Tests;

public static class TestAppInfo
{
    public static readonly AppKey AppKey = AppKey.ServiceApp("Test");
}
public sealed class TestApiFactory : AppApiFactory
{
    private readonly IServiceProvider services;

    public TestApiFactory(IServiceProvider services)
    {
        this.services = services;
    }

    protected override IAppApi _Create(IAppApiUser user)
        => new TestApi(user, services);
}
public sealed class TestApi : ConsoleAppApiWrapper
{
    public TestApi(IAppApiUser user, IServiceProvider sp)
        : base
        (
            new AppApi
            (
                TestAppInfo.AppKey,
                user,
                ResourceAccess.AllowAuthenticated()
            ),
            sp
        )
    {
        var counter = sp.GetRequiredService<Counter>();
        var options = sp.GetRequiredService<TestOptions>();
        Test = new TestGroup(source.AddGroup(nameof(Test)), counter, options);
    }

    public TestGroup Test { get; }
}

public sealed class TestGroup : AppApiGroupWrapper
{
    public TestGroup(AppApiGroup source, Counter counter, TestOptions options)
        : base(source)
    {
        RunContinuously = source.AddAction
        (
            nameof(RunContinuously),
            () => new RunContinuouslyAction(counter)
        );
        RunUntilSuccess = source.AddAction
        (
            nameof(RunUntilSuccess),
            () => new RunUntilSuccessAction(counter)
        );
        OptionalRun = source.AddAction
        (
            nameof(OptionalRun),
            () => new OptionalRunAction(counter, options)
        );
    }

    public AppApiAction<EmptyRequest, EmptyActionResult> RunContinuously { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> RunUntilSuccess { get; }
    public AppApiAction<EmptyRequest, EmptyActionResult> OptionalRun { get; }
}

public sealed class RunContinuouslyAction : AppAction<EmptyRequest, EmptyActionResult>
{
    private readonly Counter counter;

    public RunContinuouslyAction(Counter counter)
    {
        this.counter = counter;
    }

    public Task<EmptyActionResult> Execute(EmptyRequest model, CancellationToken stoppingToken)
    {
        counter.IncrementContinuous();
        return Task.FromResult(new EmptyActionResult());
    }
}

public sealed class RunUntilSuccessAction : AppAction<EmptyRequest, EmptyActionResult>
{
    private readonly Counter counter;

    public RunUntilSuccessAction(Counter counter)
    {
        this.counter = counter;
    }

    public Task<EmptyActionResult> Execute(EmptyRequest model, CancellationToken stoppingToken)
    {
        counter.IncrementUntilSuccess();
        return Task.FromResult(new EmptyActionResult());
    }
}

public sealed class TestOptions
{
    public bool IsOptional { get; set; }
    public bool ThrowException { get; set; }
}

public sealed class OptionalRunAction : OptionalAction<EmptyRequest, EmptyActionResult>
{
    private readonly Counter counter;
    private readonly TestOptions options;

    public OptionalRunAction(Counter counter, TestOptions options)
    {
        this.counter = counter;
        this.options = options;
    }

    public Task<EmptyActionResult> Execute(EmptyRequest model, CancellationToken stoppingToken)
    {
        counter.IncrementOptional();
        if (options.ThrowException)
        {
            throw new Exception("Testing");
        }
        return Task.FromResult(new EmptyActionResult());
    }

    public Task<bool> IsOptional()
    {
        return Task.FromResult(options.IsOptional);
    }
}