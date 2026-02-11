using System;
using System.Net.Http;
using System.Threading.Tasks;
using MVVM.Models.Auth;
using MVVM.Services;
using MVVM.Tools;

namespace MVVM.ViewModels;

public class LoginViewModel : BaseVM
{
    private readonly ApiService _apiService;
    private readonly AuthService _authService;
    
    private string _username = "";
    public string Username
    {
        get => _username;
        set => SetField(ref _username, value);
    }

    private string _password = "";
    public string Password
    {
        get => _password;
        set => SetField(ref _password, value);
    }

    private bool _rememberMe = false;
    public bool RememberMe
    {
        get => _rememberMe;
        set => SetField(ref _rememberMe, value);
    }

    private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetField(ref _errorMessage, value);
    }

    public RelayCommand LoginCommand { get; }
    public LoginViewModel(ApiService api, AuthService auth)
    {
        _apiService = api;
        _authService = auth;
        
        LoginCommand = new RelayCommand(async () => await LoginAsync());
    }
    
    private async Task LoginAsync()
    {
        ErrorMessage = "";
        try
        {
            var response = await _apiService.LoginAsync(new LoginDto()
            {
                Username = this.Username,
                Password = this.Password
            });

            await _authService.SaveTokenAsync(response.Token, RememberMe);
            
            //Доделать навигацию
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            ErrorMessage = "Неверный логин или пароль";
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
        }
    }
}