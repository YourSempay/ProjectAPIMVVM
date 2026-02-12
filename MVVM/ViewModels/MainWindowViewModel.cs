using System;
using System.Collections.ObjectModel;
using MVVM.Models.Employees;
using MVVM.Models.Shifts;
using MVVM.Services;
using MVVM.Tools;

namespace MVVM.ViewModels;

public partial class MainWindowViewModel : BaseVM
{
    private readonly ApiService _apiService;
    private readonly AuthService _authService;
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
    }

    public RelayCommand LogoutCommand { get; }
    public event Action? RequestOpenLoginWindow;
    public MainWindowViewModel(ApiService api, AuthService auth)
    {
        _apiService = api;
        _authService = auth;

        LogoutCommand = new RelayCommand(Logout);

        LoadData();
    }

    private async void LoadData()
    {
        var profile = await _apiService.GetProfileAsync();
        CurrentUser = $"{profile.EmployeeD.FirstName} {profile.EmployeeD.LastName}";
        CurrentRole = profile.RoleD.Title;

        var employees = await _apiService.GetEmployeesAsync();
        Employees.Clear();
        foreach (var emp in employees)
            Employees.Add(emp);

        var shifts = await _apiService.GetShiftsAsync();
        Shifts.Clear();
        foreach (var shift in shifts)
            Shifts.Add(shift);
    }

    private void Logout()
    {
        _authService.ClearTokenAsync();
        
        RequestOpenLoginWindow?.Invoke();
    }
    
}