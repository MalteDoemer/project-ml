using System.Text;
using System.Collections.Immutable;
using System.Drawing;

namespace ML;
class Program
{
    public static async Task Main()
    {
        var apiKey = File.ReadAllText("api-key.txt");
        var analyzer = new ImageAnalyzer("./res/image5.jpg", apiKey);

        var bm = new Bitmap(Image.FromFile("./res/image5.jpg"));
        var redPen = new Pen(Brushes.Red, 10.0f);
        var drawFont = new Font("Consolas", 128);

        var faces = await analyzer.GetFaces();

        using (var gr = Graphics.FromImage(bm))
        {
            foreach (var face in faces)
            {
                Console.WriteLine($"Found face at ({face.faceRectangle.left}, {face.faceRectangle.top}, {face.faceRectangle.width}, {face.faceRectangle.height})");
                var rect = new Rectangle(face.faceRectangle.left, face.faceRectangle.top, face.faceRectangle.width, face.faceRectangle.height);
                gr.DrawRectangle(redPen, rect);

                var xMiddle = rect.X + (rect.Width / 2);
                var yMiddle = rect.Y + (rect.Height / 2);

                gr.DrawString($"{face.age}", drawFont, Brushes.Red, new PointF(xMiddle, yMiddle));
            }

            gr.Save();
        }

        bm.Save("./out/image.jpg");
    }
}