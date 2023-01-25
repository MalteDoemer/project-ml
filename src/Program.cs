using System.Text;
using System.Collections.Immutable;

namespace ML;
class Program
{
    public static async Task Main()
    {
        var apiKey = File.ReadAllText("api-key.txt");
        var analyzer = new ImageAnalyzer("./res/image3.jpg", apiKey);

        var objs = analyzer.GetObjects();

        foreach (var obj in await objs)
        {
            Console.WriteLine(obj.@object);
        }
    }
}