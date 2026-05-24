using Atlas.Lite.Web.Options;

namespace Atlas.Lite.Web.Tests;

public sealed class WebOptionsValidatorTests
{
    [Fact]
    public void Validate_FailsWhenApplicationNameIsMissing()
    {
        var result = new AtlasLiteWebOptionsValidator().Validate(null, new AtlasLiteWebOptions
        {
            ApplicationName = " "
        });

        Assert.True(result.Failed);
    }

    [Fact]
    public void Validate_FailsWhenOriginIsInvalid()
    {
        var result = new AtlasLiteWebOptionsValidator().Validate(null, new AtlasLiteWebOptions
        {
            AllowedOrigins = ["ftp://example.com"]
        });

        Assert.True(result.Failed);
    }
}
