using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Services;

public class AuthService
{
    private const string FileName = "auth_token.dat"; // файл для хранения токена
    private static readonly byte[] Salt = Encoding.UTF8.GetBytes("ProjectAPIMVVMSalt"); // соль для шифрования
    //Что такое соль (Salt) и зачем она нужна:
    //Соль — это случайная или фиксированная последовательность байтов, которая добавляется к паролю перед хешированием или генерацией ключа.
    //Она защищает от атаки с таблицами радужных цепочек (rainbow tables).
    //Даже если два пользователя используют одинаковый пароль, результирующий ключ будет разным, если соль разная.
    //Я сделал фиксированную соль. Это не идеально с точки зрения криптографии (лучше случайную для каждого пользователя), но для локального шифрования токена достаточно.
    private readonly string filePath;

    // Текущий токен
    public string? Token { get; private set; }
    public bool IsAuthorized => !string.IsNullOrEmpty(Token); // проверка авторизации
    public event Action? OnTokenChanged; // событие для уведомления о смене токена
    public AuthService()
    {
        // создаём папку AppData для хранения токена
        var dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ProjectAPIMVVM");

        Directory.CreateDirectory(dir);
        filePath = Path.Combine(dir, FileName);
    }

    // Сохраняет токен в памяти и при необходимости в файл
    public async Task SaveTokenAsync(string token, bool remember)
    {
        Token = token;
        OnTokenChanged?.Invoke();   // уведомляем приложение о смене токена
        if (!remember)
            return;

        var encrypted = Encrypt(token);
        await File.WriteAllBytesAsync(filePath, encrypted);
    }
    // Загружает токен из файла при старте приложения
    public async Task LoadTokenAsync()
    {
        if (!File.Exists(filePath))
            return;

        var encrypted = await File.ReadAllBytesAsync(filePath);
        Token = Decrypt(encrypted);
        OnTokenChanged?.Invoke();
    }
    // Очистка токена (логаут)
    public Task ClearTokenAsync()
    {
        Token = null;

        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }

    // Шифрование токена
    private static byte[] Encrypt(string text)
    {
        // Создаём AES для симметричного шифрования
        using var aes = Aes.Create();
        using var key = new Rfc2898DeriveBytes(
            Environment.UserName, // имя текущего пользователя как "пароль"
            Salt,                  // соль для защиты
            10000,                 // количество итераций для PBKDF2
            HashAlgorithmName.SHA256
        );
        //Что это значит:
        //Rfc2898DeriveBytes реализует PBKDF2 — функцию, которая превращает пароль + соль в криптографический ключ.
         //   «Количество итераций» — сколько раз алгоритм повторяет вычисление хеша.
        //    Чем больше итераций, тем дольше вычислять ключ. Это защищает от брутфорса: злоумышленнику придётся делать 10 000 вычислений для каждой попытки.
         //   Здесь мы используем 10000, что достаточно для защиты токена в локальном файле.
         
         
        aes.Key = key.GetBytes(32); // 256-битный ключ
        aes.IV = key.GetBytes(16);  // 128-битный IV (инициализационный вектор)

        using var encryptor = aes.CreateEncryptor();
        var bytes = Encoding.UTF8.GetBytes(text); // текст токена в байты
        // Шифруем токен
        return encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
    }

    // Расшифровка токена
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
        // Расшифровка токена в байты
        var bytes = decryptor.TransformFinalBlock(encrypted, 0, encrypted.Length);
        // Возвращаем строку токена
        return Encoding.UTF8.GetString(bytes);
    }
}