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

        var bm = new Bitmap(Image.FromFile(file));
        var redPen = new Pen(Brushes.Red, 10.0f);
        var drawFont = new Font("Consolas", 128);

        var faces = await analyzer.GetFaces();

        using (var gr = Graphics.FromImage(bm))
        {
            foreach (var face in faces)
            {
                var rect = new Rectangle(face.faceRectangle.left, face.faceRectangle.top, face.faceRectangle.width, face.faceRectangle.height);
                gr.DrawRectangle(redPen, rect);

                var xMiddle = rect.X + (rect.Width / 2);
                var yMiddle = rect.Y + (rect.Height / 2);

                gr.DrawString($"{face.age}", drawFont, Brushes.Red, new PointF(xMiddle, yMiddle));
            }

            gr.Save();
        }

        bm.Save(output);
    }
}