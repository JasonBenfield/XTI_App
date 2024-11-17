using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using XTI_Core;
using XTI_Forms;

namespace XTI_WebAppClient;

public sealed class AppClientPostAction<TModel, TResult>
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly XtiTokenAccessor xtiTokenAccessor;
    private readonly AppClientUrl clientUrl;
    private readonly AppClientOptions options;
    private readonly string actionName;
    private static bool? cachedHasFiles;

    public AppClientPostAction(IHttpClientFactory httpClientFactory, XtiTokenAccessor xtiTokenAccessor, AppClientUrl clientUrl, AppClientOptions options, string actionName)
    {
        this.httpClientFactory = httpClientFactory;
        this.xtiTokenAccessor = xtiTokenAccessor;
        this.clientUrl = clientUrl;
        this.options = options;
        this.actionName = actionName;
    }

    public Task<string> Url() => Url("");

    public Task<string> Url(string modifier) => _Url(default, modifier);

    public Task<string> Url(TModel model) => Url(model, "");

    public Task<string> Url(TModel model, string modifier) => _Url(model, modifier);

    private async Task<string> _Url(TModel? model, string modifier)
    {
        var url = await clientUrl.Value(actionName, modifier);
        var query = new ObjectToQueryString(model).Value;
        if (!string.IsNullOrWhiteSpace(query))
        {
            query = $"?{query}";
        }
        return $"{url}{query}";
    }

    public Task<TResult> Post(string modifier, TModel model, CancellationToken ct) =>
        _Post(modifier, model, true, ct);

    private async Task<TResult> _Post(string modifier, TModel model, bool retryUnauthorized, CancellationToken ct)
    {
        var postResult = await GetPostResponse(modifier, model, ct);
        TResult result;
        try
        {
            if (postResult.IsSuccessful)
            {
                var resultContainer = JsonSerializer.Deserialize<ResultContainer<TResult>>
                (
                    postResult.Content, options.JsonSerializerOptions
                );
                if (resultContainer == null) { throw new ArgumentNullException(nameof(resultContainer)); }
                result = resultContainer.Data ?? throw new ArgumentNullException("resultContainer.Data");
            }
            else if (postResult.StatusCode == HttpStatusCode.Unauthorized && retryUnauthorized)
            {
                xtiTokenAccessor.Reset();
                result = await _Post(modifier, model, false, ct);
            }
            else
            {
                throw CreatePostException(postResult);
            }
        }
        catch (JsonException ex)
        {
            throw CreateJsonException(postResult, ex);
        }
        return result;
    }

    private async Task<PostResult> GetPostResponse(string modifier, object? model, CancellationToken ct)
    {
        PostResult postResult;
        var response = await GetPostResponseMessage(modifier, model, ct);
        try
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            postResult = new PostResult
            (
                response.IsSuccessStatusCode,
                response.StatusCode,
                responseContent,
                response.Content.Headers.ContentType?.MediaType ?? "",
                response.RequestMessage?.RequestUri?.ToString() ?? ""
            );
        }
        finally
        {
            response.Dispose();
        }
        return postResult;
    }

    private async Task<HttpResponseMessage> GetPostResponseMessage(string modifier, object? model, CancellationToken ct)
    {
        using var client = httpClientFactory.CreateClient();
        client.InitFromOptions(options);
        if (!actionName.Equals("Authenticate", StringComparison.OrdinalIgnoreCase))
        {
            var token = await xtiTokenAccessor.Value();
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        var url = await clientUrl.Value(actionName, modifier);
        if (model == null) { throw new ArgumentNullException(nameof(model)); }
        object transformedModel = model;
        if (model is Form form)
        {
            transformedModel = form.Export();
        }
        HttpContent content;
        if (!cachedHasFiles.HasValue)
        {
            cachedHasFiles = HasFiles(typeof(TModel));
        }
        if (cachedHasFiles == true)
        {
            var multiContent = new MultipartFormDataContent();
            var files = new Dictionary<string, FileUpload>();
            GetFiles(files, "", model);
            foreach (var key in files.Keys)
            {
                var file = files[key];
                if (file.Stream.Length > 0)
                {
                    var streamContent = new StreamContent(file.Stream);
                    if (!string.IsNullOrWhiteSpace(file.ContentType))
                    {
                        streamContent.Headers.ContentType = MediaTypeHeaderValue.Parse(file.ContentType);
                    }
                    multiContent.Add(streamContent, key, file.FileName);
                }
            }
            var formData = new Dictionary<string, string>();
            GetFormData(formData, "", model);
            foreach (var key in formData.Keys)
            {
                var value = formData[key];
                multiContent.Add(new StringContent(value), key);
            }
            content = multiContent;
        }
        else
        {
            var serialized = JsonSerializer.Serialize(transformedModel, options.JsonSerializerOptions);
            content = new StringContent(serialized, Encoding.UTF8, WebContentTypes.Json);
        }
        var response = await client.PostAsync(url, content, ct);
        content.Dispose();
        return response;
    }

    private static bool HasFiles(Type type)
    {
        if (type == typeof(FileUpload))
        {
            return true;
        }
        else if (type.IsArray)
        {
            return HasFiles(type.GetElementType()!);
        }
        else if (!type.IsValueType && type != typeof(string))
        {
            var properties = type
                .GetProperties()
                .Where(p => !p.GetIndexParameters().Any())
                .ToArray();
            foreach (var property in properties)
            {
                var hasFiles = HasFiles(property.PropertyType);
                if (hasFiles)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static void GetFiles(Dictionary<string, FileUpload> files, string pre, object sourceObj)
    {
        if (sourceObj is FileUpload file)
        {
            files.Add(string.IsNullOrWhiteSpace(pre) ? "model" : pre, file);
        }
        else if (sourceObj.GetType().IsArray)
        {
            var arr = (Array)sourceObj;
            var arrPre = string.IsNullOrWhiteSpace(pre) ? "model" : pre;
            foreach (var i in Enumerable.Range(0, arr.Length))
            {
                var value = arr.GetValue(i);
                if (value != null)
                {
                    GetFiles(files, $"{arrPre}[{i}]", value);
                }
            }
        }
        else if (!sourceObj.GetType().IsValueType && !(sourceObj is string))
        {
            var properties = sourceObj.GetType()
                .GetProperties()
                .Where(p => !p.GetIndexParameters().Any())
                .ToArray();
            foreach (var property in properties)
            {
                var value = property.GetValue(sourceObj);
                if (value != null)
                {
                    GetFiles
                    (
                        files,
                        string.IsNullOrWhiteSpace(pre) ? property.Name : $"{pre}.{property.Name}",
                        value
                    );
                }
            }
        }
    }

    private static void GetFormData(Dictionary<string, string> formData, string pre, object sourceObj)
    {
        if (!(sourceObj is FileUpload))
        {
            if (sourceObj is string str)
            {
                formData.Add(string.IsNullOrWhiteSpace(pre) ? "model" : pre, str);
            }
            else if (sourceObj is DateTimeOffset dateTimeOffset)
            {
                formData.Add(string.IsNullOrWhiteSpace(pre) ? "model" : pre, dateTimeOffset.ToString("O"));
            }
            else if (sourceObj is DateOnly dateOnly)
            {
                formData.Add(string.IsNullOrWhiteSpace(pre) ? "model" : pre, dateOnly.ToString("O"));
            }
            else if (sourceObj is TimeOnly timeOnly)
            {
                formData.Add(string.IsNullOrWhiteSpace(pre) ? "model" : pre, timeOnly.ToString("O"));
            }
            else if (sourceObj is TimeSpan timeSpan)
            {
                formData.Add(string.IsNullOrWhiteSpace(pre) ? "model" : pre, timeSpan.ToString("G"));
            }
            else if (sourceObj.GetType().IsValueType)
            {
                formData.Add(string.IsNullOrWhiteSpace(pre) ? "model" : pre, sourceObj.ToString() ?? "");
            }
            else if (sourceObj.GetType().IsArray)
            {
                var arr = (Array)sourceObj;
                var arrPre = string.IsNullOrWhiteSpace(pre) ? "model" : pre;
                foreach (var i in Enumerable.Range(0, arr.Length))
                {
                    var value = arr.GetValue(i);
                    if (value != null)
                    {
                        GetFormData(formData, $"{arrPre}[{i}]", value);
                    }
                }
            }
            else
            {
                var properties = sourceObj.GetType()
                    .GetProperties()
                    .Where(p => !p.GetIndexParameters().Any())
                    .ToArray();
                foreach (var property in properties)
                {
                    var value = property.GetValue(sourceObj);
                    if (value != null)
                    {
                        GetFormData
                        (
                            formData,
                            string.IsNullOrWhiteSpace(pre) ? property.Name : $"{pre}.{property.Name}",
                            value
                        );
                    }
                }
            }
        }
    }

    private static AppClientException CreatePostException(PostResult postResult)
    {
        WebErrorResult errorResult;
        if
        (
            postResult.ContentType.Equals(WebContentTypes.Json, StringComparison.OrdinalIgnoreCase) ||
            postResult.ContentType.Equals("text/json", StringComparison.OrdinalIgnoreCase)
        )
        {
            errorResult = new DeserializedWebErrorResult(postResult.Content).Value;
        }
        else
        {
            errorResult = new WebErrorResult
            (
                "", 
                AppEventSeverity.Values.CriticalError, 
                []
            );
        }
        var ex = new AppClientException
        (
            postResult.Url,
            (int)postResult.StatusCode,
            postResult.Content,
            errorResult.LogEntryKey,
            errorResult.Severity,
            errorResult.Errors
        );
        return ex;
    }

    private static AppClientException CreateJsonException(PostResult postResult, JsonException ex) =>
        new AppClientException
        (
            postResult.Url,
            (int)postResult.StatusCode,
            postResult.Content,
            "",
            AppEventSeverity.Values.CriticalError,
            [new ErrorModel(ex.Message)]
        );

    private sealed record PostResult(bool IsSuccessful, HttpStatusCode StatusCode, string Content, string ContentType, string Url);
}
