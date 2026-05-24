namespace Atlas.Lite.Core.Exceptions;

public abstract class AtlasLiteException : Exception
{
    protected AtlasLiteException(string code, string message)
        : base(message)
    {
        Code = code;
    }

    protected AtlasLiteException(string code, string message, Exception innerException)
        : base(message, innerException)
    {
        Code = code;
    }

    public string Code { get; }
}
