using Avalonia.Controls;
using MVVM.Services;
using MVVM.ViewModels;

namespace MVVM.Views;

public partial class MainWindow : Window
{
    private readonly ApiService _apiService;
    private readonly AuthService _authService;

    public MainWindow(ApiService api, AuthService auth)
    {
        InitializeComponent();

        _apiService = api;   // сохраняем ApiService
        _authService = auth; // сохраняем AuthService

        // Создаём ViewModel главного окна
        var vm = new MainWindowViewModel(api, auth);

        // Подписка на событие RequestOpenLoginWindow
        // Когда пользователь нажимает "Выйти" в главном окне, вызывается этот код:
        vm.RequestOpenLoginWindow += () =>
        {
            //  Создаём окно логина
            var loginWindow = new LoginWindow(api, auth);

            //  Показываем окно логина
            loginWindow.Show();

            //  Закрываем главное окно
            this.Close();
        };
        // Привязываем ViewModel к MainWindow
        DataContext = vm;
    }
}