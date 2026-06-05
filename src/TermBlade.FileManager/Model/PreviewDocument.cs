namespace TermBlade.FileManager;

internal readonly record struct PreviewDocument(
    bool IsText,
    string Content,
    string Language,
    int ContentWidth,
    int ContentHeight,
    string Message);
