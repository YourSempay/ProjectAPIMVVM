using System;
using System.Net.Http;
using System.Threading.Tasks;
using MVVM.Models.Auth;
using MVVM.Services;
using MVVM.Tools;

namespace MVVM.ViewModels;

public class LoginViewModel : BaseVM
{
    // Сервисы для работы с API и токенами
    private readonly ApiService _apiService;
    private readonly AuthService _authService;
    
    private string _username = "";
    public string Username
    {
        get => _username;
        set => SetField(ref _username, value);
        // SetField вызывает PropertyChanged для обновления UI
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
        // Привязано к CheckBox "Запомнить сессию" /пока не работает я хз чето с токеном нужно делать сам ебу с мозгами
    }

    private string _errorMessage = "";
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetField(ref _errorMessage, value);
        // Привязано к TextBlock для отображения ошибок
    }
    // Событие для открытия главного окна
    public event Action? RequestOpenMainWindow;
    // View подписывается на это событие, VM не знает о UI напрямую
    
    // Команда для кнопки "Войти" (кнопка привязана через Command)
    public RelayCommand LoginCommand { get; }
    public LoginViewModel(ApiService api, AuthService auth)
    {
        _apiService = api;
        _authService = auth;
        // Настраиваем команду: при нажатии кнопки выполняется LoginAsync
        LoginCommand = new RelayCommand(async () => await LoginAsync());
    }
    
    private async Task LoginAsync()
    {
        ErrorMessage = ""; // сбрасываем старую ошибку
        try
        { 
            // Отправка логина и пароля на сервер через ApiService
            var response = await _apiService.LoginAsync(new LoginDto()
            {
                Username = this.Username,
                Password = this.Password
            });
            // Сохраняем токен в AuthService
            // RememberMe — если true, токен сохраняется на диск в зашифрованном виде
            await _authService.SaveTokenAsync(response.Token, RememberMe);
            
            // Сигнал View о том, что логин успешен
            // View запсукает  RequestOpenMainWindow
            //Реализация события в LigonWindow.axaml.cs
            RequestOpenMainWindow?.Invoke();
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            // Ошибка сервера: неверный логин/пароль
            ErrorMessage = "Неверный логин или пароль";
        }
        catch (Exception e)
        {
            // Все другие ошибки (например, сеть)
            ErrorMessage = e.Message;
        }
    }
}