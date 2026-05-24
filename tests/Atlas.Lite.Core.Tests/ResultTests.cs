using Atlas.Lite.Core.Results;

namespace Atlas.Lite.Core.Tests;

public sealed class ResultTests
{
    [Fact]
    public void Result_Success_HasNoErrors()
    {
        var result = Result.Success();

        Assert.True(result.Succeeded);
        Assert.False(result.Failed);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Result_Failure_CarriesErrors()
    {
        var result = Result.Failure(new Error("invalid", "The value is invalid."));

        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.Single(result.Errors);
    }
}
