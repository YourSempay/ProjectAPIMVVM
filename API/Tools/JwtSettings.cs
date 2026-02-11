using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.Tools;

public class JwtSettings
{
    public const string ISSUER = "server data";
    public const string AUDIENCE = "client data";
    
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        // нужна строка на минимум 32 символа
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(new string('s', 32)));
}