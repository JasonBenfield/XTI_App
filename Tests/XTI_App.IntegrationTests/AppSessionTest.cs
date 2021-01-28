using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using XTI_Configuration.Extensions;
using XTI_Core;

namespace XTI_App.IntegrationTests
{
    public sealed class AppSessionTest
    {
        [Test]
        public async Task ShouldGetActiveSessions()
        {
            var services = await setup();
            var factory = services.GetService<AppFactory>();
            var user = await factory.Users().User(AppUserName.Anon);
            var createdSession = await factory.Sessions().Create
            (
                "JustCreated",
                user,
                DateTimeOffset.UtcNow,
                "Testing",
                "UserAgent",
                "127.0.0.1"
            );
            var activeSessions = (await factory.Sessions().ActiveSessions(TimeRange.OnOrBefore(DateTime.UtcNow.AddDays(1)))).ToArray();
            Assert.That(activeSessions.Length, Is.EqualTo(1), "Should include the session that was just created");
            Assert.That(activeSessions[0].HasStarted(), Is.True);
            Assert.That(activeSessions[0].HasEnded(), Is.False);
            await createdSession.End(DateTimeOffset.UtcNow);
            activeSessions = (await factory.Sessions().ActiveSessions(TimeRange.OnOrBefore(DateTime.UtcNow.AddDays(1)))).ToArray();
            Assert.That(activeSessions.Length, Is.EqualTo(0), "Should not include session after it ended");
        }

        [Test]
        public async Task ShouldGetMostRecentRequest()
        {
            var services = await setup();
            var factory = services.GetService<AppFactory>();
            var user = await factory.Users().User(AppUserName.Anon);
            var createdSession = await createSession(factory, user);
            await logRequest(services, createdSession);
            var activeSessions = (await factory.Sessions().ActiveSessions(TimeRange.OnOrBefore(DateTime.UtcNow.AddDays(1)))).ToArray();
            var requests = (await activeSessions[0].MostRecentRequests(1)).ToArray();
            Assert.That(requests.Length, Is.EqualTo(1), "Should get most recent request");
        }

        [Test]
        public async Task ShouldGetMostRecentRequestsForApp()
        {
            var services = await setup();
            var factory = services.GetService<AppFactory>();
            var user = await factory.Users().User(AppUserName.Anon);
            var createdSession = await createSession(factory, user);
            await logRequest(services, createdSession);
            var app = await services.FakeApp();
            var requests = (await app.MostRecentRequests(10)).ToArray();
            Assert.That(requests.Length, Is.EqualTo(1));
        }

        [Test]
        public async Task ShouldGetMostRecentErrorEventsForApp()
        {
            var services = await setup();
            var factory = services.GetService<AppFactory>();
            var user = await factory.Users().User(AppUserName.Anon);
            var createdSession = await createSession(factory, user);
            var request = await logRequest(services, createdSession);
            await logEvent(request);
            var app = await services.FakeApp();
            var events = (await app.MostRecentErrorEvents(10)).ToArray();
            Assert.That(events.Length, Is.EqualTo(1));
        }

        private static async Task<IServiceProvider> setup()
        {
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Test");
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration
                (
                    (hostContext, config) =>
                    {
                        config.UseXtiConfiguration(hostContext.HostingEnvironment, new string[] { });
                    }
                )
                .ConfigureServices
                (
                    (hostContext, services) =>
                    {
                        services.AddXtiTestServices(hostContext.Configuration);
                    }
                )
                .Build();
            var scope = host.Services.CreateScope();
            await scope.ServiceProvider.Reset();
            return scope.ServiceProvider;
        }

        private static Task<AppSession> createSession(AppFactory factory, AppUser user)
        {
            return factory.Sessions().Create
            (
                "JustCreated",
                user,
                DateTimeOffset.UtcNow,
                "Testing",
                "UserAgent",
                "127.0.0.1"
            );
        }

        private static async Task<AppRequest> logRequest(IServiceProvider services, AppSession createdSession)
        {
            var app = await services.FakeApp();
            var version = await app.CurrentVersion();
            var resourceGroup = await version.ResourceGroup(new ResourceGroupName("Employee"));
            var resource = await resourceGroup.Resource(new ResourceName("AddEmployee"));
            var modCategory = await app.ModCategory(ModifierCategoryName.Default);
            var modifier = await modCategory.Modifier(ModifierKey.Default);
            var request = await createdSession.LogRequest
            (
                "New-Request",
                resource,
                modifier,
                "/Fake/Current",
                DateTimeOffset.UtcNow
            );
            return request;
        }

        private static Task<AppEvent> logEvent(AppRequest request)
        {
            return request.LogEvent
            (
                new GeneratedKey().Value(),
                AppEventSeverity.Values.CriticalError,
                DateTimeOffset.Now,
                "Test",
                "Test Error",
                "Testing"
            );
        }

    }
}
