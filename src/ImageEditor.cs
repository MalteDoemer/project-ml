using System.Drawing;
using System.Runtime.Versioning;

namespace ML;

[SupportedOSPlatform("Windows")]
class ImageEditor : IDisposable
{
    private Bitmap bitmap;
    private Font font;
    private Pen redPen;
    private Graphics graphics;

    public ImageEditor(string file) : this(new Bitmap(Image.FromFile(file))) { }

    public ImageEditor(Bitmap bitmap)
    {
        this.bitmap = bitmap;
        this.graphics = Graphics.FromImage(bitmap);
        this.font = new Font("Consolas", calculateFontWeight());
        this.redPen = new Pen(Brushes.Red, calculateLineWidth());
    }

    public void DrawRectangle(Rectangle rect)
    {
        graphics.DrawRectangle(redPen, rect);
    }

    public void DrawTextInRectangle(string text, Rectangle rect)
    {
        var format = new StringFormat();
        format.Trimming = StringTrimming.None;
        format.Alignment = StringAlignment.Far;
        format.LineAlignment = StringAlignment.Far;
        graphics.DrawString(text, font, Brushes.Red, rect, format);
    }

    public void Save(string file)
    {
        bitmap.Save(file);
    }

    private float calculateLineWidth() => bitmap.Width / 256.0f;

    private int calculateFontWeight() => bitmap.Width / 32;

    public void Dispose()
    {
        graphics.Dispose();
    }
}