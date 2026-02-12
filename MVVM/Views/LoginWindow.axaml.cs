using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MVVM.Services;
using MVVM.ViewModels;

namespace MVVM.Views;

public partial class LoginWindow : Window
{
    // Ссылки на сервисы для работы с API и токенами
    private readonly ApiService _apiService;
    private readonly AuthService _authService;

    public LoginWindow(ApiService api, AuthService auth)
    {
        InitializeComponent();

        _apiService = api;   // сохраняем ссылку на ApiService
        _authService = auth; // сохраняем ссылку на AuthService

        // Создаём экземпляр LoginViewModel
        var vm = new LoginViewModel(api, auth);

        // Подписка на событие RequestOpenMainWindow, которое вызывается из VM
        // Когда логин успешен, вызывается этот код:
        vm.RequestOpenMainWindow += () =>
        {
            //  Создаём главное окно, передаём те же сервисы
            var mainWindow = new MainWindow(api, auth);

            //  Показываем главное окно
            mainWindow.Show();

            //  Закрываем окно логина
            // Это важно, чтобы окно логина не оставалось открытым
            this.Close();
        };

        // Привязываем ViewModel к окну
        // В XAML все привязки будут работать через этот DataContext
        DataContext = vm;
    }
}