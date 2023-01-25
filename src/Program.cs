using System.Text;
using System.Collections.Immutable;

namespace ML;
class Program
{
    public static async Task Main()
    {
        var apiKey = File.ReadAllText("api-key.txt");
        var analyzer = new ImageAnalyzer("./res/image4.jpg", apiKey);

        var faces = await analyzer.GetFaces();

        foreach (var face in faces)
        {
            Console.WriteLine(face.age);
        }
    }
}