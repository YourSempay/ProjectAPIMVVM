using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MVVM.Services;
using MVVM.ViewModels;

namespace MVVM.Views;

public partial class LoginWindow : Window
{
    private readonly ApiService _apiService;
    private readonly AuthService _authService;

    public LoginWindow(ApiService api, AuthService auth)
    {
        InitializeComponent();

        _apiService = api;
        _authService = auth;

        var vm = new LoginViewModel(api, auth);
        vm.RequestOpenMainWindow += () =>
        {
            var mainWindow = new MainWindow(api, auth);
            mainWindow.Show();
            this.Close();
        };
        DataContext = vm;
    }
}