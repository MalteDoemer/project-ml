using System.Text.Json;
using System.Web;
using System.Diagnostics;

namespace ML;

class ImageAnalyzer
{
    enum RequestType
    {
        Objects,
        Description,
        Categories,
        Color,
    }

    public string ImagePath { get; }

    public string ApiSubscriptionKey { get; }

    public string ApiEndpoint { get; }

    public string ApiVersion { get; }

    public ImageAnalyzer(string imagePath, string apiSubscriptionKey, string apiEndpoint = "westeurope.api.cognitive.microsoft.com", string apiVersion = "v3.0")
    {
        ImagePath = imagePath;
        ApiSubscriptionKey = apiSubscriptionKey;
        ApiEndpoint = apiEndpoint;
        ApiVersion = apiVersion;
    }

    public async Task<IReadOnlyList<Json.Object>> GetObjects()
    {
        var response = await SendRequest(RequestType.Objects);
        var json = await DeserializeResponse(response);
        Debug.Assert(json.objects is not null);
        return json.objects;
    }

    public async Task<Json.Description> GetDescription()
    {
        var response = await SendRequest(RequestType.Description);
        var json = await DeserializeResponse(response);
        Debug.Assert(json.description is not null);
        return json.description;
    }

    public async Task<IReadOnlyList<Json.Category>> GetCategories()
    {
        var response = await SendRequest(RequestType.Categories);
        var json = await DeserializeResponse(response);
        Debug.Assert(json.categories is not null);
        return json.categories;
    }

    public async Task<Json.ColorDescription> GetColor()
    {
        var response = await SendRequest(RequestType.Color);
        var json = await DeserializeResponse(response);
        Debug.Assert(json.color is not null);
        return json.color;
    }


    private Task<byte[]> GetImageBytes()
    {
        return File.ReadAllBytesAsync(ImagePath);
    }

    private string CreateUrl(RequestType requestType)
    {
        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["visualFeatures"] = requestType.ToString();
        queryString["language"] = "en";

        var url = $"https://{ApiEndpoint}/vision/{ApiVersion}/analyze?{queryString}";
        return url;
    }

    private async Task<HttpResponseMessage> SendRequest(RequestType requestType)
    {
        using var client = new HttpClient();
        var url = CreateUrl(requestType);

        var bytes = GetImageBytes();

        var content = new ByteArrayContent(await bytes);

        content.Headers.Clear();
        content.Headers.Add("Content-Type", "application/octet-stream");
        content.Headers.Add("Ocp-Apim-Subscription-Key", ApiSubscriptionKey);

        return await client.PostAsync(url, content);
    }

    private async Task<Json.Root> DeserializeResponse(HttpResponseMessage response)
    {
        var json = await JsonSerializer.DeserializeAsync<Json.Root>(await response.Content.ReadAsStreamAsync());
        Debug.Assert(json is not null);
        return json!;
    }
}