using API.DB;
using API.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Swagger для тестирования
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// добавляем контроллеры и так же делаем что json не важно большая или маленькая буква, в swagg работает,а в программе нет хз почему или я чето не так делал писао Пушкин
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNameCaseInsensitive = true);
// подключаем EF Core к БД
builder.Services.AddDbContext<_1135AlexandroContext>();
// Настройка JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // проверять Issuer
            ValidIssuer = JwtSettings.ISSUER,  
            ValidateAudience = true,  // проверять Audience
            ValidAudience = JwtSettings.AUDIENCE,
            ValidateLifetime = true,    // проверять срок жизни
            IssuerSigningKey = JwtSettings.GetSymmetricSecurityKey(), // ключ подписи
            ValidateIssuerSigningKey = true
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// включаем аутентификацию
app.UseAuthentication();
app.UseAuthorization();
// подключаем контроллеры
app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}