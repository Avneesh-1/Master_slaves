using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using MasterSlaves.Core.Models;
using MasterSlaves.Core.Services;

namespace MasterSlaves.Avalonia.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly IMasterService _masterService;
    private readonly IConfigurationService _configurationService;
    private SlaveUnit? _selectedSlave;
    private SlaveDetailViewModel? _slaveDetailViewModel;
    private bool _isAlarmActive;
    private bool _isSimulationRunning;
    private bool _isHardwareConnected;
    private string _connectionStatus = "Disconnected";
    private string _raspberryPiIp = "Auto-Discover";
    private bool _isMenuOpen = false;
    private bool _isSettingsOpen = false;
    private SettingsViewModel? _settingsViewModel;

    public MainWindowViewModel(IMasterService masterService, IConfigurationService configurationService)
    {
        _masterService = masterService;
        _configurationService = configurationService;
        
        // Initialize commands
        SelectSlaveCommand = new RelayCommand<int>(SelectSlave);
        BackToMasterCommand = new RelayCommand(BackToMaster);
        StartHardwareCommunicationCommand = new RelayCommand(StartHardwareCommunication);
        StopHardwareCommunicationCommand = new RelayCommand(StopHardwareCommunication);
        OpenMenuCommand = new RelayCommand(OpenMenu);
        CloseMenuCommand = new RelayCommand(CloseMenu);
        OpenSettingsCommand = new RelayCommand(OpenSettings);
        BackFromSettingsCommand = new RelayCommand(BackFromSettings);
        BuzzerAlarmCommand = new RelayCommand(ResetAlarm);
        
        // Initialize slave buttons
        SlaveButtons = new ObservableCollection<SlaveButtonViewModel>();
        InitializeSlaveButtons();
        
        // Subscribe to events
        _masterService.AlarmStateChanged += OnAlarmStateChanged;
        _masterService.SlaveDataUpdated += OnSlaveDataUpdated;
        
        // Load configuration
        _ = InitializeConfigurationAsync();
        
        // Simulation disabled - will be replaced with real hardware data
        IsSimulationRunning = false;
        
        // Auto-start hardware communication
        StartHardwareCommunication();
    }

    public ObservableCollection<SlaveButtonViewModel> SlaveButtons { get; }
    
    public SlaveUnit? SelectedSlave
    {
        get => _selectedSlave;
        set
        {
            if (_selectedSlave != value)
            {
                _selectedSlave = value;
                OnPropertyChanged(nameof(SelectedSlave));
                OnPropertyChanged(nameof(IsSlaveSelected));
                
                // Create or update slave detail view model
                if (value != null)
                {
                    SlaveDetailViewModel = new SlaveDetailViewModel(_masterService, _configurationService, value, BackToMasterCommand);
                }
                else
                {
                    SlaveDetailViewModel = null;
                }
            }
        }
    }

    public SlaveDetailViewModel? SlaveDetailViewModel
    {
        get => _slaveDetailViewModel;
        set
        {
            if (_slaveDetailViewModel != value)
            {
                _slaveDetailViewModel = value;
                OnPropertyChanged(nameof(SlaveDetailViewModel));
            }
        }
    }

    public bool IsSlaveSelected => SelectedSlave != null;

    public bool IsAlarmActive
    {
        get => _isAlarmActive;
        set
        {
            if (_isAlarmActive != value)
            {
                _isAlarmActive = value;
                OnPropertyChanged(nameof(IsAlarmActive));
            }
        }
    }

    public bool IsSimulationRunning
    {
        get => _isSimulationRunning;
        set
        {
            if (_isSimulationRunning != value)
            {
                _isSimulationRunning = value;
                OnPropertyChanged(nameof(IsSimulationRunning));
            }
        }
    }

    public bool IsHardwareConnected
    {
        get => _isHardwareConnected;
        set
        {
            if (_isHardwareConnected != value)
            {
                _isHardwareConnected = value;
                OnPropertyChanged(nameof(IsHardwareConnected));
            }
        }
    }

    public string ConnectionStatus
    {
        get => _connectionStatus;
        set
        {
            if (_connectionStatus != value)
            {
                _connectionStatus = value;
                OnPropertyChanged(nameof(ConnectionStatus));
            }
        }
    }

    public string RaspberryPiIp
    {
        get => _raspberryPiIp;
        set
        {
            if (_raspberryPiIp != value)
            {
                _raspberryPiIp = value;
                OnPropertyChanged(nameof(RaspberryPiIp));
                _configurationService.SetRaspberryPiIpAddress(value);
                _ = _configurationService.SaveConfigurationAsync();
            }
        }
    }

    public bool IsMenuOpen
    {
        get => _isMenuOpen;
        set
        {
            if (_isMenuOpen != value)
            {
                _isMenuOpen = value;
                OnPropertyChanged(nameof(IsMenuOpen));
            }
        }
    }

    public bool IsSettingsOpen
    {
        get => _isSettingsOpen;
        set
        {
            if (_isSettingsOpen != value)
            {
                _isSettingsOpen = value;
                OnPropertyChanged(nameof(IsSettingsOpen));
            }
        }
    }

    public SettingsViewModel? SettingsViewModel
    {
        get => _settingsViewModel;
        set
        {
            if (_settingsViewModel != value)
            {
                _settingsViewModel = value;
                OnPropertyChanged(nameof(SettingsViewModel));
            }
        }
    }

    // Commands
    public ICommand SelectSlaveCommand { get; }
    public ICommand BackToMasterCommand { get; }
    public ICommand StartHardwareCommunicationCommand { get; }
    public ICommand StopHardwareCommunicationCommand { get; }
    public ICommand OpenMenuCommand { get; }
    public ICommand CloseMenuCommand { get; }
    public ICommand OpenSettingsCommand { get; }
    public ICommand BackFromSettingsCommand { get; }
    public ICommand BuzzerAlarmCommand { get; }

    private void InitializeSlaveButtons()
    {
        RefreshSlaveButtons();
    }

    private void RefreshSlaveButtons()
    {
        SlaveButtons.Clear();
        var slaves = _masterService.GetAllSlaves();
        
        foreach (var slave in slaves)
        {
            var customName = _configurationService.GetSlaveName(slave.Id);
            var button = new SlaveButtonViewModel(slave, customName);
            SlaveButtons.Add(button);
        }
    }

    private async Task InitializeConfigurationAsync()
    {
        try
        {
            await _configurationService.LoadConfigurationAsync();
            RaspberryPiIp = _configurationService.GetRaspberryPiIpAddress();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load configuration: {ex.Message}");
        }
    }

    private void SelectSlave(int slaveId)
    {
        SelectedSlave = _masterService.GetSlaveById(slaveId);
    }



    private void BackToMaster()
    {
        SelectedSlave = null;
    }

    private async void StartHardwareCommunication()
    {
        try
        {
            // Connect to Raspberry Pi using configured IP address
            await _masterService.StartHardwareCommunicationAsync(
                _configurationService.GetRaspberryPiIpAddress(), 
                _configurationService.GetRaspberryPiPort());
        }
        catch (Exception ex)
        {
            // Handle connection error
            System.Diagnostics.Debug.WriteLine($"Hardware connection failed: {ex.Message}");
            ConnectionStatus = $"Connection failed: {ex.Message}";
        }
    }

    private async void StopHardwareCommunication()
    {
        try
        {
            await _masterService.StopHardwareCommunicationAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Hardware disconnection failed: {ex.Message}");
        }
    }

    private void OnAlarmStateChanged(object? sender, bool isAlarmActive)
    {
        IsAlarmActive = isAlarmActive;
    }

    private void OnSlaveDataUpdated(object? sender, SlaveUnit slave)
    {
        // Force UI update when slave data changes
        Console.WriteLine($"UI Update: Slave {slave.Id} data updated");
        
        // Debug: Check all gas readings
        Console.WriteLine("=== UI Gas Values ===");
        foreach (var reading in slave.AllGasReadings)
        {
            Console.WriteLine($"{reading.GasType}: {reading.Value} {reading.GasType.GetUnit()}, Flow: {reading.FlowValue}");
        }
        Console.WriteLine("=====================");
        
        // Trigger property change notifications
        OnPropertyChanged(nameof(SlaveButtons));
        
        // Update selected slave if it's the one that changed
        if (SelectedSlave?.Id == slave.Id)
        {
            OnPropertyChanged(nameof(SelectedSlave));
        }
    }

    private void OpenMenu()
    {
        IsMenuOpen = true;
    }

    private void CloseMenu()
    {
        IsMenuOpen = false;
    }

    private void OpenSettings()
    {
        // Create settings view model if not exists
        if (SettingsViewModel == null)
        {
            SettingsViewModel = new SettingsViewModel(_configurationService, BackFromSettingsCommand);
        }
        
        // Close menu and open settings
        IsMenuOpen = false;
        IsSettingsOpen = true;
    }

    private void BackFromSettings()
    {
        IsSettingsOpen = false;
        // Refresh slave buttons with new prefix
        RefreshSlaveButtons();
    }

    private void ResetAlarm()
    {
        try
        {
            Console.WriteLine(new string('=', 60));
            Console.WriteLine("🔔 BUZZER ALARM BUTTON PRESSED IN C# APP!");
            Console.WriteLine(new string('=', 60));
            
            // Send JSON command to Python server
            var alarmResetCommand = new
            {
                command = "reset_alarm"
            };
            var jsonCommand = System.Text.Json.JsonSerializer.Serialize(alarmResetCommand);
            Console.WriteLine($"📤 Sending command: '{jsonCommand}' to Python server");
            
            _masterService.SendCommandToHardware(jsonCommand);
            
            Console.WriteLine("✅ Command sent successfully to Python server");
            Console.WriteLine(new string('=', 60));
            
            System.Diagnostics.Debug.WriteLine("Alarm reset command sent to Python server");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR: Failed to send alarm reset command: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Failed to send alarm reset command: {ex.Message}");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class SlaveButtonViewModel : INotifyPropertyChanged
{
    private readonly SlaveUnit _slave;
    private readonly string _displayName;

    public SlaveButtonViewModel(SlaveUnit slave, string displayName)
    {
        _slave = slave;
        _displayName = displayName;
        _slave.PropertyChanged += OnSlavePropertyChanged;
    }

    public int Id => _slave.Id;
    public string Name => _slave.Name;
    public string DisplayName => _displayName;
    public bool IsConnected => _slave.IsConnected;
    public bool HasAlarm => _slave.AllGasReadings.Any(r => r.IsAlarmActive);

    private void OnSlavePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SlaveUnit.IsConnected) || 
            e.PropertyName == nameof(SlaveUnit.AllGasReadings))
        {
            OnPropertyChanged(nameof(IsConnected));
            OnPropertyChanged(nameof(HasAlarm));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class RelayCommand : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) => _execute();
}

public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Func<T, bool>? _canExecute;

    public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke((T)parameter!) ?? true;

    public void Execute(object? parameter) => _execute((T)parameter!);
}
