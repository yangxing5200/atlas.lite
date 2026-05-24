namespace Atlas.Lite.Core.Exceptions;

public sealed class ConflictException : AtlasLiteException
{
    public ConflictException(string message)
        : base("conflict", message)
    {
    }
}
