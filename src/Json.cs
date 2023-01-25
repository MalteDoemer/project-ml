
namespace ML.Json;

public record Category(string name, double score);

public record ColorDescription(string dominantColorForeground, string dominantColorBackground, IReadOnlyList<string> dominantColors, string accentColor, bool isBwImg);

public record Caption(string text, double confidence);

public record Description(IReadOnlyList<string> tags, IReadOnlyList<Caption> captions);

public record Metadata(int height, int width, string format);

public record Object(Rectangle? rectangle, string @object, double confidence, Object? parent);

public record Rectangle(int x, int y, int w, int h);

public record Root(Description? description, ColorDescription? color, IReadOnlyList<Object>? objects, IReadOnlyList<Category>? categories, string requestId, Metadata metadata);
