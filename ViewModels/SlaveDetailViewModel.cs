using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using MasterSlaves.Core.Models;
using MasterSlaves.Core.Services;

namespace MasterSlaves.Avalonia.ViewModels;

public class SlaveDetailViewModel : INotifyPropertyChanged
{
    private readonly IMasterService _masterService;
    private readonly SlaveUnit _slave;
    private int _selectedGasMode;

    public SlaveDetailViewModel(IMasterService masterService, SlaveUnit slave, ICommand backToMasterCommand)
    {
        _masterService = masterService;
        _slave = slave;
        
        // Initialize commands
        MuteCommand = new RelayCommand(ToggleMute);
        SettingsCommand = new RelayCommand(OpenSettings);
        FlowCommand = new RelayCommand(ToggleFlowView);
        SetGasModeCommand = new RelayCommand<int>(SetGasMode);
        BackToMasterCommand = backToMasterCommand;
        
        // Subscribe to slave property changes
        _slave.PropertyChanged += OnSlavePropertyChanged;
        
        // Initialize gas mode
        SelectedGasMode = _slave.GasMode;
    }

    public SlaveUnit Slave => _slave;
    
    public string SlaveName => _slave.Name;
    
    public bool IsMuted
    {
        get => _slave.IsMuted;
        set
        {
            if (_slave.IsMuted != value)
            {
                _slave.IsMuted = value;
                OnPropertyChanged(nameof(IsMuted));
            }
        }
    }

    public int SelectedGasMode
    {
        get => _selectedGasMode;
        set
        {
            if (_selectedGasMode != value)
            {
                _selectedGasMode = value;
                OnPropertyChanged(nameof(SelectedGasMode));
                _masterService.SetSlaveGasMode(_slave.Id, value);
            }
        }
    }

    public bool IsFlowViewActive => _slave.IsFlowViewActive;

    public ObservableCollection<GasReading> VisibleGasReadings => _slave.VisibleGasReadings;

    public ObservableCollection<GasModeOption> GasModeOptions { get; } = new()
    {
        new GasModeOption(1, "1 Gas"),
        new GasModeOption(2, "2 Gas"),
        new GasModeOption(3, "3 Gas"),
        new GasModeOption(4, "4 Gas"),
        new GasModeOption(5, "5 Gas"),
        new GasModeOption(6, "6 Gas")
    };

    // Commands
    public ICommand MuteCommand { get; }
    public ICommand SettingsCommand { get; }
    public ICommand FlowCommand { get; }
    public ICommand SetGasModeCommand { get; }
    public ICommand BackToMasterCommand { get; }

    private void ToggleMute()
    {
        _masterService.ToggleSlaveMute(_slave.Id);
    }

    private void OpenSettings()
    {
        // TODO: Implement settings dialog
    }

    private void ToggleFlowView()
    {
        _masterService.ToggleSlaveFlowView(_slave.Id);
        OnPropertyChanged(nameof(IsFlowViewActive));
    }

    private void SetGasMode(int gasMode)
    {
        SelectedGasMode = gasMode;
    }

    private void OnSlavePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SlaveUnit.IsMuted))
        {
            OnPropertyChanged(nameof(IsMuted));
        }
        else if (e.PropertyName == nameof(SlaveUnit.GasMode))
        {
            SelectedGasMode = _slave.GasMode;
        }
        else if (e.PropertyName == nameof(SlaveUnit.IsFlowViewActive))
        {
            OnPropertyChanged(nameof(IsFlowViewActive));
        }
        else if (e.PropertyName == nameof(SlaveUnit.VisibleGasReadings))
        {
            OnPropertyChanged(nameof(VisibleGasReadings));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class GasModeOption
{
    public int Mode { get; }
    public string DisplayName { get; }

    public GasModeOption(int mode, string displayName)
    {
        Mode = mode;
        DisplayName = displayName;
    }
} 