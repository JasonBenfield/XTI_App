using XTI_WebApp.Abstractions;

namespace XTI_WebApp.Fakes;

public sealed class FakeLoginReturnKey : ILoginReturnKey
{
    private readonly string value;

    public FakeLoginReturnKey(string requesterKey = "", string value = "")
    {
        this.value = string.IsNullOrWhiteSpace(value) ? 
            Guid.NewGuid().ToString("N") : 
            value;
    }

    public Task<string> Value(string requesterKey, string returnUrl) => Task.FromResult(value);
}
