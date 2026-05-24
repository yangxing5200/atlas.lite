namespace Atlas.Lite.Core.Exceptions;

public sealed class NotFoundException : AtlasLiteException
{
    public NotFoundException(string resourceName, object key)
        : base("not_found", $"{resourceName} with key '{key}' was not found.")
    {
        ResourceName = resourceName;
        Key = key;
    }

    public string ResourceName { get; }

    public object Key { get; }
}
