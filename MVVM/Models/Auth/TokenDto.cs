namespace MVVM.Models.Auth;

public class TokenDto
{
    public string Token { get; set; }
    public int ExpiresIn { get; set; }
}