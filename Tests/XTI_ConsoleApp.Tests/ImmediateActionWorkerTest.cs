using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace XTI_ConsoleApp.Tests;

public sealed class ImmediateActionWorkerTest
{
    private static readonly Counter counter = new();

    [Test]
    public async Task ShouldRunImmediateAction()
    {
        var hostBuilder = BuildHost();
        var host = hostBuilder.Build();
        await host.RunAsync();
        Assert.That(counter.ContinuousValue, Is.EqualTo(1));
    }

    private IHostBuilder BuildHost()
    {
        return Host.CreateDefaultBuilder([])
            .ConfigureServices((hostContext, services) =>
            {
                services.AddTestConsoleAppServices
                (
                    (sp, b) =>
                    {
                        b.AddImmediate<TestApi>(api => api.Test.RunContinuously);
                        b.AddImmediate<TestApi>(api => api.Lifetime.StopApplication);
                    }
                );
                services.AddSingleton(_ => counter);
            });
    }
}