using System.Text.Json;
using System.Text;
using System.Web;

namespace ML;

public class Metadata
{
    public int height { get; set; }
    public int width { get; set; }
    public string? format { get; set; }
}

public class Object
{
    public Rectangle? rectangle { get; set; }
    public string? @object { get; set; }
    public double confidence { get; set; }
    public Parent? parent { get; set; }
}

public class Parent
{
    public string? @object { get; set; }
    public double confidence { get; set; }
}

public class Rectangle
{
    public int x { get; set; }
    public int y { get; set; }
    public int w { get; set; }
    public int h { get; set; }
}

public class Root
{
    public List<Object>? objects { get; set; }
    public string? requestId { get; set; }
    public Metadata? metadata { get; set; }
}


class Program
{
    public static async Task Main()
    {
        using var client = new HttpClient();

        var queryString = HttpUtility.ParseQueryString(string.Empty);
        queryString["visualFeatures"] = "Objects";
        queryString["language"] = "en";

        var version = "v3.0";
        var endpoint = "westeurope.api.cognitive.microsoft.com";
        var url = $"https://{endpoint}/vision/{version}/analyze?{queryString}";

        var content = new StringContent("{\"url\":\"https://www.supervision-systems.ch/div/testbild.jpg\"}", Encoding.UTF8, "application/json");

        content.Headers.Clear();
        content.Headers.Add("Content-Type", "application/json");
        content.Headers.Add("Ocp-Apim-Subscription-Key", "ddb26f4091604a8ab5a2872a82403846");

        var res = await client.PostAsync(url, content);

        var json = await JsonSerializer.DeserializeAsync<Root>(await res.Content.ReadAsStreamAsync());

        printObject(json!.objects!.First());
    }

    static void printObject(ML.Object obj)
    {
        Console.WriteLine($"The AI has detected a {obj.@object} with {obj.confidence} confidence.");
    }

}