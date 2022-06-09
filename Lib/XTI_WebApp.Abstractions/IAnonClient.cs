namespace XTI_WebApp.Abstractions;

public interface IAnonClient
{
    string RequesterKey { get; }
    string SessionKey { get; }
    DateTimeOffset SessionExpirationTime { get; }

    void Load();
    void Persist(string sessionKey, DateTimeOffset sessionExpirationTime, string requesterKey);
}