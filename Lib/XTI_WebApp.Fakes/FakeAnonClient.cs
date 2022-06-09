using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Fakes;

public sealed class FakeAnonClient : IAnonClient
{
    public string RequesterKey { get; private set; } = "";

    public string SessionKey { get; private set; } = "";

    public DateTimeOffset SessionExpirationTime { get; private set; }

    public void Load()
    {
    }

    public void Persist(string sessionKey, DateTimeOffset sessionExpirationTime, string requesterKey)
    {
        SessionKey = sessionKey;
        SessionExpirationTime = sessionExpirationTime;
        RequesterKey = requesterKey;
    }
}