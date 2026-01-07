namespace XTI_WebApp.Api;

public interface ITransformedLink
{
    Task<LinkModel> Value(CancellationToken ct);
}
