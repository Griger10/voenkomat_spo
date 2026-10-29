using VoenkomatSpo.Infra.Authentication;
using Xunit;

namespace VoenkomatSpo.Tests;

public sealed class PasswordHasherTests
{
    private readonly Pbkdf2PasswordHasher hasher = new();

    [Fact]
    public void HashUsesDistinctSaltsAndVerifiesPasswordExactly()
    {
        var first = hasher.Hash("Тест! 123");
        Assert.NotEqual(first, hasher.Hash("Тест! 123"));
        Assert.True(hasher.Verify("Тест! 123", first));
        Assert.False(hasher.Verify("Тест! 123 ", first));
        Assert.False(hasher.Verify("wrong", first));
    }

    [Fact]
    public void ExistingExampleHashIsCompatible()
    {
        const string seeded = "pbkdf2-sha256$210000$UvhEHvGA1zr0UWe2sKX1tA==$H0LumF6PFzbaSvYC3K8wu7utWyWZEFaopelvvTWCszI=";
        Assert.True(hasher.Verify("DemoOnly!2026", seeded));
        Assert.False(hasher.Verify("demoOnly!2026", seeded));
    }

    [Theory]
    [InlineData("")]
    [InlineData("plain-text")]
    [InlineData("pbkdf2-sha256$-1$bad$bad")]
    [InlineData("pbkdf2-sha256$2147483647$bad$bad")]
    [InlineData("pbkdf2-sha256$210000$bad$bad")]
    [InlineData("pbkdf2-sha256$210000$YQ==$YQ==")]
    public void MalformedHashFailsWithoutThrowing(string hash) => Assert.False(hasher.Verify("password", hash));
}
