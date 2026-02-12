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

        _apiService = api;
        _authService = auth;

        var vm = new MainWindowViewModel(api, auth);
        vm.RequestOpenLoginWindow += () =>
        {
            var loginWindow = new LoginWindow(api, auth);
            loginWindow.Show();
            this.Close();
        };

        DataContext = vm;
    }
}