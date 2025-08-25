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
    private readonly IConfigurationService _configurationService;
    private readonly SlaveUnit _slave;

    public SlaveDetailViewModel(IMasterService masterService, IConfigurationService configurationService, SlaveUnit slave, ICommand backToMasterCommand)
    {
        _masterService = masterService;
        _configurationService = configurationService;
        _slave = slave;
        
        // Initialize commands
        MuteCommand = new RelayCommand(ToggleMute);
        SettingsCommand = new RelayCommand(OpenSettings);
        FlowCommand = new RelayCommand(ToggleFlowView);
        BackToMasterCommand = backToMasterCommand;
        
        // Subscribe to slave property changes
        _slave.PropertyChanged += OnSlavePropertyChanged;
    }

    public SlaveUnit Slave => _slave;
    
    public string SlaveName => _configurationService.GetSlaveName(_slave.Id);
    
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



    public bool IsFlowViewActive => _slave.IsFlowViewActive;

    public ObservableCollection<GasReading> VisibleGasReadings => _slave.VisibleGasReadings;

    // Commands
    public ICommand MuteCommand { get; }
    public ICommand SettingsCommand { get; }
    public ICommand FlowCommand { get; }
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



    private void OnSlavePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SlaveUnit.IsMuted))
        {
            OnPropertyChanged(nameof(IsMuted));
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