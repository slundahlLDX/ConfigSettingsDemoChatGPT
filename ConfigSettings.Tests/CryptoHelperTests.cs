using ConfigSettings.Shared.Crypto;
using Xunit;

namespace ConfigSettings.Tests;

public class CryptoHelperTests
{
    [Fact]
    public void EncryptDecrypt_RoundTrip_ReturnsOriginalText()
    {
        var master = "unit-test-master-key-1234567890";
        var crypto = new CryptoHelper(master, "abc");
        var salt = CryptoHelper.GenerateSalt();
        var iv = CryptoHelper.GenerateIV();
        var plain = "Hello-Unit-Test";

        var cipher = crypto.Encrypt(plain, salt, iv);
        var result = crypto.Decrypt(cipher, salt, iv);

        Assert.NotEqual(plain, cipher);
        Assert.Equal(plain, result);
    }
}
