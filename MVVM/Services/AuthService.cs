using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Services;

public class AuthService
{
    private const string FileName = "auth_token.dat";
    private static readonly byte[] Salt = Encoding.UTF8.GetBytes("ProjectAPIMVVMSalt");

    private readonly string filePath;

    public string? Token { get; private set; }
    public bool IsAuthorized => !string.IsNullOrEmpty(Token);
    public event Action? OnTokenChanged; 
    public AuthService()
    {
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ProjectAPIMVVM");

        Directory.CreateDirectory(dir);
        filePath = Path.Combine(dir, FileName);
    }

    public async Task SaveTokenAsync(string token, bool remember)
    {
        Token = token;
        OnTokenChanged?.Invoke(); 
        if (!remember)
            return;

        var encrypted = Encrypt(token);
        await File.WriteAllBytesAsync(filePath, encrypted);
    }

    public async Task LoadTokenAsync()
    {
        if (!File.Exists(filePath))
            return;

        var encrypted = await File.ReadAllBytesAsync(filePath);
        Token = Decrypt(encrypted);
        OnTokenChanged?.Invoke();
    }

    public Task ClearTokenAsync()
    {
        Token = null;

        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }

    private static byte[] Encrypt(string text)
    {
        using var aes = Aes.Create();
        using var key = new Rfc2898DeriveBytes(
            Environment.UserName,
            Salt,
            10000,
            HashAlgorithmName.SHA256
        );

        aes.Key = key.GetBytes(32);
        aes.IV = key.GetBytes(16);

        using var encryptor = aes.CreateEncryptor();
        var bytes = Encoding.UTF8.GetBytes(text);

        return encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
    }

    private static string Decrypt(byte[] encrypted)
    {
        using var aes = Aes.Create();
        using var key = new Rfc2898DeriveBytes(
            Environment.UserName,
            Salt,
            10000,
            HashAlgorithmName.SHA256
        );

        aes.Key = key.GetBytes(32);
        aes.IV = key.GetBytes(16);

        using var decryptor = aes.CreateDecryptor();
        var bytes = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);

        return Encoding.UTF8.GetString(bytes);
    }
}