using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using XTI_App.Abstractions;
using XTI_App.Api;
using XTI_App.Extensions;
using XTI_App.Fakes;
using XTI_Core;
using XTI_Core.Extensions;
using XTI_Core.Fakes;
using XTI_TempLog;
using XTI_TempLog.Abstractions;
using XTI_WebApp.Abstractions;
using XTI_WebApp.Api;
using XTI_WebApp.Extensions;
using XTI_WebApp.Fakes;

namespace XTI_WebApp.AspTests;

#pragma warning disable CS0162
internal sealed class SessionLogMiddlewareTest
{
    [Test]
    public async Task ShouldCreateSession()
    {
        var input = await Setup();
        await input.GetAsync("/Fake/Current/Controller1/Action1");
        var session = await GetFirstStartSession(input);
        Assert.That
        (
            session.UserAgent,
            Is.EqualTo("Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.52 Safari/537.36 OPR/15.0.1147.100"),
            "Should create session with user agent from request"
        );
    }

    [Test]
    public async Task ShouldLogRequesterWithSession()
    {
        var input = await Setup();
        await input.GetAsync("/Fake/Current/Controller1/Action1");
        var session = await GetFirstStartSession(input);
        Assert.That(string.IsNullOrWhiteSpace(session.RequesterKey), Is.False, "Should log requester with session");
    }

    private static async Task<TempLogSessionModel> GetFirstStartSession(TestInput input)
    {
        var sessions = await GetSessions(input.Host.Services);
        Assert.That(sessions.Length, Is.EqualTo(1), "Should create session");
        return sessions[0];
    }

    [Test]
    public async Task ShouldReuseRequesterKeyWithNewSession()
    {
        var input = await Setup();
        await input.GetAsync("/Fake/Current/Controller1/Action1");
        input.Clock.Set(input.Clock.Now().AddHours(4).AddMinutes(1));
        await input.GetAsync("/Fake/Current/Controller1/Action1");
        var sessions = await GetSessions(input.Host.Services);
        Assert.That(sessions.Length, Is.EqualTo(2));
        Assert.That(sessions[1].RequesterKey, Is.EqualTo(sessions[0].RequesterKey), "Should reuse requester key");
    }

    [Test]
    public async Task ShouldNotStartAnonSession_WhenAnonSessionWasAlreadyStarted()
    {
        var input = await Setup();
        await input.GetAsync("/Fake/Current/Controller1/Action1");
        await input.GetAsync("/Fake/Current/Controller1/Action1");
        var sessions = await GetSessions(input.Host.Services);
        Assert.That(sessions.Length, Is.EqualTo(1), "Should not start new session when anon session has already started");
    }

    [Test]
    public async Task ShouldNotReuseAnonymousSession_WhenSessionHasExpired()
    {
        var input = await Setup();
        await input.GetAsync("/Fake/Current/Controller1/Action1");
        input.Clock.Set(input.Clock.Now().AddHours(4).AddMinutes(1));
        await input.GetAsync("/Fake/Current/Controller1/Action1");
        var sessions = await GetSessions(input.Host.Services);
        Assert.That(sessions.Length, Is.EqualTo(2), "Should not reuse anonymous session when it has expired");
        Assert.That(sessions[1].SessionKey, Is.Not.EqualTo(sessions[0].SessionKey), "Should not reuse anonymous session when it has expired");
    }

    [Test]
    public async Task ShouldCreateSessionWithAnonymousUser()
    {
        var input = await Setup();
        input.UserContext.SetCurrentUser(AppUserName.Anon);
        await input.GetAsync("/Fake/Current/Controller1/Action1");
        var anonUser = await input.UserContext.User(AppUserName.Anon);
        var sessions = await GetSessions(input.Host.Services);
        Assert.That(sessions.Length, Is.EqualTo(1), "Should use session for authenticated user");
        Assert.That(sessions[0].SessionKey.UserName, Is.EqualTo(""), "Should create session with anonymous user");
    }

    [Test]
    public async Task ShouldLogRequest()
    {
        var input = await Setup();
        var uri = "/Fake/Current/Controller1/Action1";
        await input.GetAsync(uri);
        var request = await GetStartRequest(input);
        Assert.That(request.Path, Is.EqualTo(uri), "Should log resource key for request");
    }

    [Test]
    public async Task ShouldLogEndOfRequest()
    {
        var input = await Setup();
        var uri = "/Fake/Current/Controller1/Action1";
        await input.GetAsync(uri);
        var requests = await GetEndRequests(input.Host.Services);
        Assert.That(requests.Length, Is.EqualTo(1), "Should log end of request");
    }

    [Test]
    public async Task ShouldLogCurrentVersionWithRequest()
    {
        var input = await Setup();
        var uri = "/Fake/Current/Controller1/Action1";
        await input.GetAsync(uri);
        var request = await GetStartRequest(input);
        var path = XtiPath.Parse(request.Path);
        Assert.That(path.Version, Is.EqualTo(AppVersionKey.Current), "Should log current version");
    }

    [Test]
    public async Task ShouldLogExplicitVersionWithRequest()
    {
        var input = await Setup();
        var explicitVersion = input.AppContext.GetCurrentApp().Version;
        var uri = $"/Fake/{explicitVersion.VersionKey.Value}/Controller1/Action1";
        await input.GetAsync(uri);
        var request = await GetStartRequest(input);
        var path = XtiPath.Parse(request.Path);
        Assert.That(path.Version, Is.EqualTo(explicitVersion.VersionKey), "Should log explicit version");
    }

    private static async Task<TempLogRequestModel> GetStartRequest(TestInput input)
    {
        var requests = await GetStartRequests(input.Host.Services);
        return requests.FirstOrDefault() ?? new();
    }

    [Test]
    public async Task ShouldLogUnexpectedError()
    {
        Exception? exception = null;
        var input = await Setup();
        input.CurrentAction.Configure
        (
            c =>
           {
               try
               {
                   throw new Exception("Testing critical error");
               }
               catch (Exception ex)
               {
                   exception = ex;
                   throw;
               }
               return Task.CompletedTask;
           }
        );
        var uri = "/Fake/Current/Controller1/Action1";
        await input.GetAsync(uri);
        var logEntry = await GetLogEntry(input);
        Assert.That(logEntry.Severity, Is.EqualTo(AppEventSeverity.Values.CriticalError.Value), "Should log critical error");
        Assert.That(logEntry.Caption, Is.EqualTo("An unexpected error occurred"), "Should log critical error");
        Assert.That(logEntry.Message, Is.EqualTo(exception?.Message), "Should log critical error");
        Assert.That(logEntry.Detail, Is.EqualTo(exception?.StackTrace), "Should log critical error");
    }

    private static async Task<LogEntryModel> GetLogEntry(TestInput input)
    {
        var logEntries = await GetLogEntries(input.Host.Services);
        Assert.That(logEntries.Length, Is.EqualTo(1), "Should log event");
        return logEntries[0];
    }

    [Test]
    public async Task ShouldLogValidationError()
    {
        Exception? exception = null;
        var input = await Setup();
        input.CurrentAction.Configure
        (
            c =>
            {
                try
                {
                    throw new ValidationFailedException(new[] { new ErrorModel("User name is required") });
                }
                catch (Exception ex)
                {
                    exception = ex;
                    throw;
                }
                return Task.CompletedTask;
            }
        );
        var uri = "/Fake/Current/Controller1/Action1";
        await input.GetAsync(uri);
        var evt = await GetLogEntry(input);
        Assert.That(evt.Severity, Is.EqualTo(AppEventSeverity.Values.ValidationFailed.Value), "Should log validation failed");
        Assert.That(evt.Caption, Is.EqualTo("Validation Failed"), "Should log validation failed");
        Assert.That(evt.Message, Is.EqualTo("Validation failed with the following errors:\r\nUser name is required"), "Should log validation failed");
        Assert.That(evt.Detail, Is.EqualTo(exception?.StackTrace), "Should log validation failed");
    }

    [Test]
    public async Task ShouldLogAppError()
    {
        Exception? exception = null;
        var input = await Setup();
        input.CurrentAction.Configure
        (
            c =>
            {
                try
                {
                    throw new AppException("Full Message", "Display Message");
                }
                catch (Exception ex)
                {
                    exception = ex;
                    throw;
                }
                return Task.CompletedTask;
            }
        );
        var uri = "/Fake/Current/Controller1/Action1";
        await input.GetAsync(uri);
        var evt = await GetLogEntry(input);
        Assert.That(evt.Severity, Is.EqualTo(AppEventSeverity.Values.AppError.Value), "Should log app error");
        Assert.That(evt.Caption, Is.EqualTo("Display Message"), "Should log app error");
        Assert.That(evt.Message, Is.EqualTo("Full Message"), "Should log app error");
        Assert.That(evt.Detail, Is.EqualTo(exception?.StackTrace), "Should log app error");
    }

    [Test]
    public async Task ShouldLogAccessDenied()
    {
        Exception? exception = null;
        var uri = "/Fake/Current/Controller1/Action1";
        var input = await Setup();
        input.CurrentAction.Configure
        (
            c =>
            {
                try
                {
                    throw new AccessDeniedException($"Access denied to {uri}");
                }
                catch (Exception ex)
                {
                    exception = ex;
                    throw;
                }
                return Task.CompletedTask;
            }
        );
        await input.GetAsync(uri);
        var evt = await GetLogEntry(input);
        Assert.That(evt.Severity, Is.EqualTo(AppEventSeverity.Values.AccessDenied.Value), "Should log access denied");
        Assert.That(evt.Caption, Is.EqualTo("Access Denied"), "Should log access denied");
        Assert.That(evt.Message, Is.EqualTo("Access denied to /Fake/Current/Controller1/Action1"), "Should log access denied");
        Assert.That(evt.Detail, Is.EqualTo(exception?.StackTrace), "Should log access denied");
    }

    [Test]
    public async Task ShouldReturnServerError_WhenAnUnexpectedErrorOccurs()
    {
        Exception? exception = null;
        const string envName = "Production";
        var input = await Setup(envName);
        input.CurrentAction.Configure
        (
            c =>
            {
                try
                {
                    throw new Exception("Testing critical error");
                }
                catch (Exception ex)
                {
                    exception = ex;
                    throw;
                }
                return Task.CompletedTask;
            }
        );
        var uri = $"/Fake/Current/Controller1/Action1?cacheBust={input.AppContext.GetCurrentApp().Version.VersionKey.DisplayText}";
        var response = await input.GetAsync(uri);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        var result = await response.Content.ReadAsStringAsync();
        var evt = await GetLogEntry(input);
        var expectedResult = JsonSerializer.Serialize
        (
            new ResultContainer<WebErrorResult>
            (
                new WebErrorResult
                (
                    evt.EventKey,
                    AppEventSeverity.Values.Value(evt.Severity),
                    new[] { new ErrorModel("An unexpected error occurred") }
                )
            )
        );
        Assert.That(result, Is.EqualTo(expectedResult), "Should return errors");
    }

    [Test]
    public async Task ShouldReturnBadRequestError400_WhenAValidationErrorOccurs()
    {
        var errors = new[] { new ErrorModel("Field is required") };
        var input = await Setup();
        input.CurrentAction.Configure
        (
            c =>
            {
                throw new ValidationFailedException(errors);
            }
        );
        var uri = "/Fake/Current/Controller1/Action1";
        var response = await input.GetAsync(uri);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var result = await response.Content.ReadAsStringAsync();
        var evt = await GetLogEntry(input);
        var expectedResult = JsonSerializer.Serialize
        (
            new ResultContainer<WebErrorResult>
            (
                new WebErrorResult(evt.EventKey, AppEventSeverity.Values.Value(evt.Severity), errors)
            )
        );
        Assert.That(result, Is.EqualTo(expectedResult), "Should return errors");
    }

    [Test]
    public async Task ShouldReturnForbiddenError403_WhenAValidationErrorOccurs()
    {
        AccessDeniedException? exception = null;
        const string envName = "Production";
        var input = await Setup(envName);
        var uri = $"/Fake/Current/Controller1/Action1?cacheBust={input.AppContext.GetCurrentApp().Version.VersionKey.DisplayText}";
        input.CurrentAction.Configure
        (
            c =>
            {
                try
                {
                    c.Request.Method = "POST";
                    c.Request.ContentType = WebContentTypes.Json;
                    throw new AccessDeniedException("Access denied");
                }
                catch (AccessDeniedException ex)
                {
                    exception = ex;
                    throw;
                }
                return Task.CompletedTask;
            }
        );
        var response = await input.GetAsync(uri);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        var result = await response.Content.ReadAsStringAsync();
        var errors = new[] { new ErrorModel(exception?.DisplayMessage ?? "") };
        var evt = await GetLogEntry(input);
        var expectedResult = JsonSerializer.Serialize
        (
            new ResultContainer<WebErrorResult>
            (
                new WebErrorResult
                (
                    evt.EventKey,
                    AppEventSeverity.Values.Value(evt.Severity),
                    errors
                )
            )
        );
        Assert.That(result, Is.EqualTo(expectedResult), "Should return errors");
    }

    [Test]
    public async Task ShouldReturnDisplayMessage_WhenAnAppErrorOccurs()
    {
        TestAppException? exception = null;
        const string envName = "Production";
        var input = await Setup(envName);
        var uri = $"/Fake/Current/Controller1/Action1t?cacheBust={input.AppContext.GetCurrentApp().Version.VersionKey.DisplayText}";
        input.CurrentAction.Configure
        (
            c =>
            {
                try
                {
                    throw new TestAppException();
                }
                catch (TestAppException ex)
                {
                    exception = ex;
                    throw;
                }
                return Task.CompletedTask;
            }
        );
        var response = await input.GetAsync(uri);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var result = await response.Content.ReadAsStringAsync();
        var errors = new[] { new ErrorModel(exception?.DisplayMessage ?? "") };
        var evt = await GetLogEntry(input);
        var expectedResult = JsonSerializer.Serialize
        (
            new ResultContainer<WebErrorResult>
            (
                new WebErrorResult
                (
                    evt.EventKey,
                    AppEventSeverity.Values.Value(evt.Severity),
                    errors
                )
            )
        );
        Assert.That(result, Is.EqualTo(expectedResult), "Should return errors");
    }

    private sealed class FakeRequestData
    {
        public string Arg1 { get; set; } = "";
    }

    [Test]
    public async Task ShouldLogout()
    {
        const string envName = "Production";
        var input = await Setup(envName);
        var uri = $"/Fake/Current/User/Logout?cacheBust={input.AppContext.GetCurrentApp().Version.VersionKey.DisplayText}";
        input.CurrentAction.Configure
        (
            async c =>
            {
                var action = c.RequestServices.GetRequiredService<LogoutAction>();
                await action.Execute(new LogoutRequest(""), CancellationToken.None);
            }
        );
        var response = await input.GetAsync(uri);
        var tempLog = input.Host.Services.GetRequiredService<TempLog>();
        var clock = input.Host.Services.GetRequiredService<IClock>();
        var endSessions = await GetEndSessions(input.Host.Services);
        Assert.That
        (
            endSessions.Length,
            Is.EqualTo(1),
            "Should end session"
        );
    }

    private sealed class TestAppException : AppException
    {
        public TestAppException() : base("Detailed message", "Message for user")
        {
        }
    }

    private sealed class CurrentAction
    {
        public CurrentAction()
        {
            Action = (c) =>
           {
               c.Response.StatusCode = StatusCodes.Status200OK;
               c.Request.Method = "POST";
               c.Request.ContentType = WebContentTypes.Json;
               return Config(c);
           };
        }
        public TempLog? TempLog { get; set; }
        public Func<HttpContext, Task> Action { get; }
        private Func<HttpContext, Task> Config { get; set; } = (_) => Task.CompletedTask;

        public void Configure(Func<HttpContext, Task> config)
        {
            Config = config;
        }
    }

    private async Task<TestInput> Setup(string envName = "Test")
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", envName);
        var hostBuilder = new HostBuilder();
        var appKey = FakeInfo.AppKey;
        var host = await hostBuilder
            .ConfigureAppConfiguration
            (
                (hostingContext, config) =>
                {
                    config.UseXtiConfiguration(hostingContext.HostingEnvironment, appKey.Name.DisplayText, appKey.Type.DisplayText, new string[0]);
                }
            )
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureServices((context, services) =>
                    {
                        services.AddSingleton(_ => XtiEnvironment.Parse(envName));
                        services.AddSingleton<CurrentAction>();
                        services.AddSingleton<TestAuthOptions>();
                        services.AddSingleton<XtiAuthenticationOptions>();
                        services
                            .AddAuthentication("Test")
                            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>
                            (
                                "Test",
                                options => { }
                            );
                        services.AddAuthorization(options =>
                        {
                            options.DefaultPolicy =
                                new AuthorizationPolicyBuilder("Test")
                                    .RequireAuthenticatedUser()
                                    .Build();
                        });
                        services.AddFakesForXtiWebApp();
                        services.AddScoped<XtiRequestContext>();
                        services.AddSingleton<FakeAppContext>();
                        services.AddSingleton<ISourceAppContext>(sp => sp.GetRequiredService<FakeAppContext>());
                        services.AddSingleton<CachedAppContext>();
                        services.AddSingleton<IAppContext>(sp => sp.GetRequiredService<CachedAppContext>());
                        services.AddSingleton<FakeUserContext>();
                        services.AddSingleton<ISourceUserContext>(sp => sp.GetRequiredService<FakeUserContext>());
                        services.AddSingleton<CachedUserContext>();
                        services.AddSingleton<IUserContext>(sp => sp.GetRequiredService<CachedUserContext>());
                        services.AddSingleton(sp => FakeInfo.AppKey);
                        services.AddSingleton<FakeAppOptions>();
                        services.AddScoped<FakeAppApiFactory>();
                        services.AddScoped<AppApiFactory>(sp => sp.GetRequiredService<FakeAppApiFactory>());
                        services.AddSingleton<IAnonClient, FakeAnonClient>();
                        services.AddScoped<FakeAppSetup>();
                        services.AddMvc();
                    })
                    .Configure(app =>
                    {
                        app.UseAuthentication();
                        app.UseAuthorization();
                        app.UseXti();
                        app.Run(async (c) =>
                        {
                            var currentAction = c.RequestServices.GetRequiredService<CurrentAction>();
                            currentAction.TempLog = c.RequestServices.GetRequiredService<TempLog>();
                            await currentAction.Action(c);
                        });
                    });
            })
            .StartAsync();
        var authOptions = host.Services.GetRequiredService<XtiAuthenticationOptions>();
        authOptions.JwtSecret = "JwtSecret";
        var setup = host.Services.GetRequiredService<FakeAppSetup>();
        await setup.Run(AppVersionKey.Current);
        var userContext = host.Services.GetRequiredService<FakeUserContext>();
        userContext.AddUser(new AppUserName("xartogg"));
        return new TestInput(host);
    }

    private static async Task<TempLogSessionModel[]> GetSessions(IServiceProvider services)
    {
        var logFiles = await WriteLogFiles(services);
        var sessionDetails = await logFiles[0].Read();
        return sessionDetails.Select(s => s.Session).ToArray();
    }

    private static async Task<TempLogSessionModel[]> GetEndSessions(IServiceProvider services)
    {
        var logFiles = await WriteLogFiles(services);
        var sessionDetails = await logFiles[0].Read();
        return sessionDetails.Select(s => s.Session).Where(s => s.TimeEnded.Year < 9999).ToArray();
    }

    private static async Task<TempLogRequestModel[]> GetStartRequests(IServiceProvider services)
    {
        var logFiles = await WriteLogFiles(services);
        var sessionDetails = await logFiles[0].Read();
        var sessions = sessionDetails.Select(s => s.Session).ToArray();
        var requests = sessionDetails
            .SelectMany(sd => sd.RequestDetails.Select(rd => rd.Request))
            .OrderBy(r => r.TimeStarted)
            .ToArray();
        return requests;
    }

    private static async Task<TempLogRequestModel[]> GetEndRequests(IServiceProvider services)
    {
        var logFiles = await WriteLogFiles(services);
        var sessionDetails = await logFiles[0].Read();
        var sessions = sessionDetails.Select(s => s.Session).ToArray();
        var requests = sessionDetails
            .SelectMany(sd => sd.RequestDetails.Select(rd => rd.Request))
            .Where(r => r.TimeEnded.Year < 9999)
            .OrderBy(r => r.TimeStarted)
            .ToArray();
        return requests;
    }

    private static async Task<LogEntryModel[]> GetLogEntries(IServiceProvider services)
    {
        var logFiles = await WriteLogFiles(services);
        var sessionDetails = await logFiles[0].Read();
        var sessions = sessionDetails.Select(s => s.Session).ToArray();
        var logEntries = sessionDetails
            .SelectMany(sd => sd.RequestDetails.SelectMany(rd => rd.LogEntries))
            .OrderBy(r => r.TimeOccurred)
            .ToArray();
        return logEntries;
    }

    private static async Task<ITempLogFile[]> WriteLogFiles(IServiceProvider sp)
    {
        var tempLogRepo = sp.GetRequiredService<TempLogRepository>();
        await tempLogRepo.WriteToLocalStorage();
        var clock = sp.GetRequiredService<IClock>();
        var tempLog = sp.GetRequiredService<TempLog>();
        var logFiles = tempLog.Files(clock.Now().AddSeconds(1), 100);
        return logFiles;
    }

    private sealed class TestInput
    {
        public TestInput(IHost host)
        {
            Host = host;
            Clock = (FakeClock)host.Services.GetRequiredService<IClock>();
            TestAuthOptions = host.Services.GetRequiredService<TestAuthOptions>();
            Cookies = new CookieContainer();
            CurrentAction = host.Services.GetRequiredService<CurrentAction>();
            AppContext = host.Services.GetRequiredService<FakeAppContext>();
            UserContext = host.Services.GetRequiredService<FakeUserContext>();
        }
        public IHost Host { get; }
        public CookieContainer Cookies { get; }
        public FakeClock Clock { get; }
        public TestAuthOptions TestAuthOptions { get; }
        public CurrentAction CurrentAction { get; }
        public FakeAppContext AppContext { get; }
        public FakeUserContext UserContext { get; }

        public async Task<HttpResponseMessage> GetAsync(string relativeUrl)
        {
            var testServer = Host.GetTestServer();
            testServer.BaseAddress = new Uri("https://localhost");
            var absoluteUrl = new Uri(testServer.BaseAddress, relativeUrl);
            var requestBuilder = testServer.CreateRequest(absoluteUrl.ToString());
            requestBuilder.AddHeader(HeaderNames.Authorization, "Test");
            requestBuilder.AddHeader(HeaderNames.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.52 Safari/537.36 OPR/15.0.1147.100");
            AddCookies(requestBuilder, absoluteUrl);
            var response = await requestBuilder.GetAsync();
            UpdateCookies(response, absoluteUrl);
            return response;
        }

        public async Task<HttpResponseMessage> PostAsync(string relativeUrl, object data)
        {
            var testServer = Host.GetTestServer();
            testServer.BaseAddress = new Uri("https://localhost");
            var absoluteUrl = new Uri(testServer.BaseAddress, relativeUrl);
            var requestBuilder = testServer.CreateRequest(absoluteUrl.ToString());
            requestBuilder.AddHeader(HeaderNames.Authorization, "Test");
            requestBuilder.AddHeader(HeaderNames.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.52 Safari/537.36 OPR/15.0.1147.100");
            AddCookies(requestBuilder, absoluteUrl);
            requestBuilder.And
            (
                r => r.Content = new StringContent(XtiSerializer.Serialize(data), System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json"))
            );
            var response = await requestBuilder.PostAsync();
            UpdateCookies(response, absoluteUrl);
            return response;
        }

        private void AddCookies(RequestBuilder requestBuilder, Uri absoluteUrl)
        {
            var cookieHeader = Cookies.GetCookieHeader(absoluteUrl);
            if (!string.IsNullOrWhiteSpace(cookieHeader))
            {
                requestBuilder.AddHeader(HeaderNames.Cookie, cookieHeader);
            }
        }

        private void UpdateCookies(HttpResponseMessage response, Uri absoluteUrl)
        {
            if (response.Headers.Contains(HeaderNames.SetCookie))
            {
                var cookies = response.Headers.GetValues(HeaderNames.SetCookie);
                foreach (var cookie in cookies)
                {
                    Cookies.SetCookies(absoluteUrl, cookie);
                }
            }
        }
    }
}