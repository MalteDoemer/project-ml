
namespace ML.Json;

public record Face(int age, string gender, FaceRectangle faceRectangle);

public record Category(string name, double score);

public record ImageType(int clipArtType, int lineDrawingType);

public record ColorDescription(string dominantColorForeground, string dominantColorBackground, IReadOnlyList<string> dominantColors, string accentColor, bool isBwImg);

public record Caption(string text, double confidence);

public record Tag(string name, double confidence);

public record Description(IReadOnlyList<string> tags, IReadOnlyList<Caption> captions);

public record Metadata(int height, int width, string format);

public record Object(Rectangle? rectangle, string @object, double confidence, Object? parent);

public record Rectangle(int x, int y, int w, int h);

public record FaceRectangle(int left, int top, int width, int height);

public record Root(Description? description, ImageType? imageType, ColorDescription? color, IReadOnlyList<Tag>? tags, IReadOnlyList<Object>? objects, IReadOnlyList<Face>? faces, IReadOnlyList<Category>? categories, string requestId, Metadata metadata);
