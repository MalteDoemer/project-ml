using System.Text;
using System.Collections.Immutable;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.Versioning;
using Mono.Options;

namespace ML;
class Program
{
    [SupportedOSPlatform("Windows")]
    public static async Task Main(string[] args)
    {

        string? output = null;
        string? file = null;
        string apiKeyFile = "api-key.txt";

        var options = new OptionSet() {
            { "o|output=", "specify the output file", value => output = value },
            { "api-key=", "specify a file which holds the api-key", value => apiKeyFile = value },
            { "<>", value => file = value }
        };

        try
        {
            options.Parse(args);
        }
        catch (OptionException e)
        {
            Console.WriteLine($"project-ml: {e.Message}");
            System.Environment.Exit(1);
        }

        if (file is null)
        {
            Console.WriteLine($"project-ml: input file must be specified");
            System.Environment.Exit(1);
        }

        if (output is null)
        {
            Console.WriteLine($"project-ml: output must be specified");
            System.Environment.Exit(1);
        }

        var apiKey = File.ReadAllText(apiKeyFile);
        var analyzer = new ImageAnalyzer(file, apiKey);
        var editor = new ImageEditor(file);

        var faces = await analyzer.GetFaces();

        foreach (var face in faces)
        {
            var rect = new Rectangle(face.faceRectangle.left, face.faceRectangle.top, face.faceRectangle.width, face.faceRectangle.height);
            editor.DrawRectangle(rect);
            editor.DrawTextInRectangle(face.age.ToString(), rect);
        }

        editor.Save(output);
    }
}