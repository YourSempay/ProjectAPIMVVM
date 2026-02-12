using System;
using System.Collections.ObjectModel;
using MVVM.Models.Employees;
using MVVM.Models.Shifts;
using MVVM.Services;
using MVVM.Tools;

namespace MVVM.ViewModels;

public partial class MainWindowViewModel : BaseVM
{
    private readonly ApiService _apiService;  // сервис для общения с API
    private readonly AuthService _authService; // сервис для управления токеном
    public ObservableCollection<EmployeeRoleDto> Employees { get; } = new();
    public ObservableCollection<ShiftEmployeeDto> Shifts { get; } = new();

    private string _currentUser = "";
    public string CurrentUser
    {
        get => _currentUser;
        set => SetField(ref _currentUser, value);
    }

    private string _currentRole = "";
    public string CurrentRole
    {
        get => _currentRole;
        set => SetField(ref _currentRole, value);
        // SetField вызывает PropertyChanged для уведомления UI об изменении
    }

    public RelayCommand LogoutCommand { get; }
    // Событие для открытия окна логина после логаута
    public event Action? RequestOpenLoginWindow;
    public MainWindowViewModel(ApiService api, AuthService auth)
    {
        _apiService = api;  // Сохраняем ссылки на сервисы
        _authService = auth;
        // Настраиваем команду LogoutCommand
        // Когда пользователь нажимает кнопку "Выйти", вызывается метод Logout
        LogoutCommand = new RelayCommand(Logout);
        // Загружаем данные из API сразу после создания ViewModel
        LoadData();
    }

    private async void LoadData()
    {
        //  Получаем профиль текущего пользователя
        var profile = await _apiService.GetProfileAsync();

        //  Формируем строку с именем и фамилией для UI
        CurrentUser = $"{profile.EmployeeD.FirstName} {profile.EmployeeD.LastName}";

        //  Получаем роль для UI
        CurrentRole = profile.RoleD.Title;

        //  Получаем список сотрудников с сервера
        var employees = await _apiService.GetEmployeesAsync();

        //  Очищаем старый список и добавляем новые данные
        Employees.Clear();
        foreach (var emp in employees)
            Employees.Add(emp); // ObservableCollection уведомляет UI о каждом добавлении

        //  Аналогично для смен
        var shifts = await _apiService.GetShiftsAsync();
        Shifts.Clear();
        foreach (var shift in shifts)
            Shifts.Add(shift);
    }

    private void Logout()
    {
        //  Удаляем токен из памяти и файла, чтобы пользователь был деавторизован
        _authService.ClearTokenAsync();
        
        //  Вызываем событие для открытия окна логина
        // реализаци этого событие находится в MainWindow.axaml.cs
        RequestOpenLoginWindow?.Invoke();
    }
    
}