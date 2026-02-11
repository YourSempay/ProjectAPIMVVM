using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MVVM.Services;
using MVVM.ViewModels;

namespace MVVM.Views;

public partial class LoginWindow : Window
{
    public LoginWindow(ApiService api, AuthService auth)
    {
        InitializeComponent();
        DataContext = new LoginViewModel(api, auth);
    }
}