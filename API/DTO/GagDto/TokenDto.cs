namespace API.DTO.GagDto;

public class TokenDto
{
    public string Token { get; set; } = null!;
    public int ExpiresIn { get; set; }
}