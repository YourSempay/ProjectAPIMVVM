using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.Tools;

public class JwtSettings
{
    public const string ISSUER = "server data";  // кто выдал токен
    public const string AUDIENCE = "client data"; // кто принимает токен
    
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        // ключ для подписи токена — минимум 32 символа
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(new string('s', 32)));
}