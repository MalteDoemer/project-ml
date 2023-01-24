using System.Text.Json;
using System.Text;
using System.Web;

namespace ML
{
    struct ImageRectangle
    {
        public ImageRectangle(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public int x { get; }
        public int y { get; }
        public int width { get; }
        public int height { get; }
    }

    struct ImageObject
    {
        public string Name { get; }
        public double Confidence { get; }
        public ImageRectangle? Rectangle { get; }

        public ImageObject(string name, double confidence, ImageRectangle? rectangle)
        {
            Name = name;
            Confidence = confidence;
            Rectangle = rectangle;
        }
    }

    class ImageAnalyzer
    {
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

        public async IAsyncEnumerable<ImageObject> GetObjects()
        {
            var response = await SendRequest("Objects");
            var jsonObjects = await DeserializeResponse(response);

            foreach (var obj in jsonObjects)
            {
                yield return ConvertObject(obj);
            }
        }

        private Task<byte[]> GetImageBytes()
        {
            return File.ReadAllBytesAsync(ImagePath);
        }

        private string GetUrl(string visualFeatures)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["visualFeatures"] = visualFeatures;
            queryString["language"] = "en";

            var url = $"https://{ApiEndpoint}/vision/{ApiVersion}/analyze?{queryString}";
            return url;
        }

        private async Task<HttpResponseMessage> SendRequest(string visualFeatures)
        {
            using var client = new HttpClient();
            var url = GetUrl(visualFeatures);

            var bytes = GetImageBytes();

            var content = new ByteArrayContent(await bytes);

            content.Headers.Clear();
            content.Headers.Add("Content-Type", "application/octet-stream");
            content.Headers.Add("Ocp-Apim-Subscription-Key", ApiSubscriptionKey);

            return await client.PostAsync(url, content);
        }

        private async Task<List<Json.Object>> DeserializeResponse(HttpResponseMessage response)
        {
            var json = await JsonSerializer.DeserializeAsync<Json.Root>(await response.Content.ReadAsStreamAsync());

            if (json is not null && json.objects is not null)
                return json.objects;
            else
                return new List<Json.Object>();
        }

        private ImageObject ConvertObject(Json.Object obj)
        {
            var name = obj.@object ?? "(Nothing)";
            var confidence = obj.confidence;

            if (obj.rectangle is not null)
            {
                var rect = new ImageRectangle(obj.rectangle.x, obj.rectangle.y, obj.rectangle.w, obj.rectangle.h);
                return new ImageObject(name, confidence, rect);
            }
            else
            {
                return new ImageObject(name, confidence, null);
            }
        }
    }

    class Program
    {
        public static async Task Main()
        {
            var apiKey = File.ReadAllText("api-key.txt");

            var analyzer = new ImageAnalyzer("./res/index.jpg", apiKey);

            await foreach (var obj in analyzer.GetObjects())
            {
                Console.WriteLine($"The AI has detected a {obj.Name} with {obj.Confidence} confidence.");
            }
        }
    }
}