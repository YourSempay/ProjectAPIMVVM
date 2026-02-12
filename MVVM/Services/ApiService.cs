using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MVVM.Models.Auth;
using MVVM.Models.Employees;
using MVVM.Models.Shifts;
using MVVM.Tools;

namespace MVVM.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    
    // Событие для уведомления о 401 Unauthorized
    public event Action? OnUnauthorized;

    public ApiService(AuthService authService)
    {
        _authService = authService;
        
        // Настройка HttpClient с базовым адресом API - адрес находишь либо в поисковой строке когда запускаешь api или в файле Api.http в проекте APi
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5093")
        };
        // Подписываемся на событие изменения токена
        _authService.OnTokenChanged += ApplyAuth;
        // Когда токен изменился, мы применяем его к HttpClient
        
        ApplyAuth(); // Применяем токен сразу при старте
    }
    
    // Устанавливает Authorization header с токеном, если он есть
    private void ApplyAuth()
    {
        // Сбрасываем старый Authorization
        _httpClient.DefaultRequestHeaders.Authorization = null;
        // Если токен есть, добавляем в заголовок Bearer
        if (_authService.IsAuthorized)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authService.Token);
            // Заголовок будет выглядеть как:
            // Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI...
        }
        
    }
    // Универсальная обработка ответов с телом
    
    private async Task<T> HandleResponse<T>(HttpResponseMessage response)
    {
        // Если сервер вернул 401, вызываем событие
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            OnUnauthorized?.Invoke();  // уведомляем приложение о 401
            throw new ApiException(HttpStatusCode.Unauthorized, "Unauthorized");
        }

        response.EnsureSuccessStatusCode();   // Выбрасываем исключение для других ошибок 4xx/5xx
                                              // Что это делает:
                                              // Любой HTTP-запрос возвращает код состояния:
                                              // 200 OK → успешно
                                              // 401 Unauthorized → неавторизован
                                              // 404 Not Found → ресурс не найден
                                              // 500 Internal Server Error → ошибка сервера
                                              // EnsureSuccessStatusCode() проверяет, что код в диапазоне 200–299.
                                              // Если код не успешный (например, 400 или 500), этот метод выбрасывает исключение HttpRequestException.
                                              // то удобно, потому что не надо вручную проверять каждый статус-код — мы сразу попадаем в catch блок в LoginViewModel.
        
        return await response.Content.ReadFromJsonAsync<T>();   // Читаем тело ответа и десериализуем JSON в T
    
    }

    // Универсальная обработка ответов без тела
    private async Task HandleResponse(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            OnUnauthorized?.Invoke();
            throw new ApiException(HttpStatusCode.Unauthorized, "Unauthorized");
        }

        response.EnsureSuccessStatusCode();
        await Task.CompletedTask;
    }
    
    // LoginAsync — отправка логина/пароля на сервер
    public async Task<TokenDto> LoginAsync(LoginDto dto)
    {
        // POST-запрос к серверу на login
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);
        // Если 401 — выбрасываем ApiException
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new ApiException(HttpStatusCode.Unauthorized, "Unauthorized");

        // Убеждаемся, что 200 OK
        response.EnsureSuccessStatusCode();
        // Читаем тело ответа как TokenDto (с токеном и временем жизни)
        return await response.Content.ReadFromJsonAsync<TokenDto>();
    }
    // Получение профиля текущего пользователя
    public async Task<EmployeeRoleDto> GetProfileAsync()
    {
        ApplyAuth();
        var response = await _httpClient.PostAsync("api/auth/profile", null);
        return await HandleResponse<EmployeeRoleDto>(response);
    }
    // Получение списка сотрудников
    public async Task<List<EmployeeRoleDto>> GetEmployeesAsync()
    {
        ApplyAuth();
        var response = await _httpClient.GetAsync("api/employees");
        
        return await HandleResponse<List<EmployeeRoleDto>>(response);
    }
    // Получение списка смен
    public async Task<List<ShiftEmployeeDto>> GetShiftsAsync()
    {
        ApplyAuth();
        var response = await _httpClient.GetAsync("api/shifts");
        return await HandleResponse<List<ShiftEmployeeDto>>(response);
    }
    
}