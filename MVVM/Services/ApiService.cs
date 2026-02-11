using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MVVM.Models.Auth;
using MVVM.Models.Employees;
using MVVM.Tools;

namespace MVVM.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    
    public event Action? OnUnauthorized;

    public ApiService(AuthService authService)
    {
        _authService = authService;
        
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:5093")
        };
        _authService.OnTokenChanged += ApplyAuth;
        ApplyAuth(); 
    }
    
    private void ApplyAuth()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;

        if (_authService.IsAuthorized)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authService.Token);
        }
        
    }
    
    private async Task<T> HandleResponse<T>(HttpResponseMessage response)
    {
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            OnUnauthorized?.Invoke();
            throw new ApiException(HttpStatusCode.Unauthorized, "Unauthorized");
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    
    }

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
    
    public async Task<TokenDto> LoginAsync(LoginDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new ApiException(HttpStatusCode.Unauthorized, "Unauthorized");

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TokenDto>();
    }

    public async Task<EmployeeRoleDto> GetProfileAsync()
    {
        ApplyAuth();
        var response = await _httpClient.PostAsync("api/auth/profile", null);
        return await HandleResponse<EmployeeRoleDto>(response);
    }
}