using System.Security.Cryptography;
using System.Text;

namespace ConfigSettings.Shared.Crypto;

public sealed class CryptoHelper
{
    private readonly string _masterKey;
    private readonly string _pepper;     // Application-specific addition to salt

    public CryptoHelper(string masterKey, string pepper)
    {
        if (string.IsNullOrEmpty(masterKey)) throw new ArgumentNullException(nameof(masterKey));
        _masterKey = masterKey;
        _pepper = pepper;

    }

    public string Encrypt(string plainText, string salt, byte[] iv)
    {
        string seasoning = salt + _pepper;

        if (string.IsNullOrEmpty(plainText)) return plainText;
        using var aes = Aes.Create();
        //aes.Key = DeriveKey(_masterKey, salt);
        aes.Key = DeriveKey(_masterKey, seasoning);
        aes.IV = iv;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs, Encoding.UTF8))
        {
            sw.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public string Decrypt(string cipherText, string salt, byte[] iv)
    {
        if (string.IsNullOrEmpty(cipherText)) return cipherText;
        using var aes = Aes.Create();
        aes.Key = DeriveKey(_masterKey, salt);
        aes.IV = iv;

        var buffer = Convert.FromBase64String(cipherText);
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(buffer);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs, Encoding.UTF8);
        return sr.ReadToEnd();
    }

    private static byte[] DeriveKey(string key, string salt)
    {
        using var derive = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes(salt), 10000, HashAlgorithmName.SHA256);
        return derive.GetBytes(32);
    }

    public static string GenerateSalt()
    {
        var saltBytes = RandomNumberGenerator.GetBytes(16);
        return Convert.ToBase64String(saltBytes);
    }

    public static byte[] GenerateIV()
    {
        return RandomNumberGenerator.GetBytes(16); // 128-bit IV for AES
    }

    public static string IVToBase64(byte[] iv) => Convert.ToBase64String(iv);
    public static byte[] IVFromBase64(string base64) => Convert.FromBase64String(base64);
}
