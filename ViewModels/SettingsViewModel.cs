using System;
using System.ComponentModel;
using System.Windows.Input;
using MasterSlaves.Core.Services;

namespace MasterSlaves.Avalonia.ViewModels;

public class SettingsViewModel : INotifyPropertyChanged
{
    private readonly IConfigurationService _configurationService;
    private string _slaveButtonPrefix = "INPUT P";
    private string _previewText = "INPUT P1, INPUT P2, INPUT P3...";

    public SettingsViewModel(IConfigurationService configurationService, ICommand backCommand)
    {
        _configurationService = configurationService;
        BackCommand = backCommand;
        SaveCommand = new RelayCommand(SaveSettings);
        ApplyPrefixCommand = new RelayCommand(ApplyPrefix);
        
        // Load current settings
        LoadSettings();
    }

    public string SlaveButtonPrefix
    {
        get => _slaveButtonPrefix;
        set
        {
            if (_slaveButtonPrefix != value)
            {
                _slaveButtonPrefix = value;
                OnPropertyChanged(nameof(SlaveButtonPrefix));
                UpdatePreview();
            }
        }
    }

    public string PreviewText
    {
        get => _previewText;
        set
        {
            if (_previewText != value)
            {
                _previewText = value;
                OnPropertyChanged(nameof(PreviewText));
            }
        }
    }

    public ICommand BackCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ApplyPrefixCommand { get; }

    private void LoadSettings()
    {
        try
        {
            // Load saved prefix from configuration
            var savedPrefix = _configurationService.GetSlaveButtonPrefix();
            if (!string.IsNullOrEmpty(savedPrefix))
            {
                SlaveButtonPrefix = savedPrefix;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load settings: {ex.Message}");
        }
    }

    private void ApplyPrefix()
    {
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        var prefix = string.IsNullOrWhiteSpace(_slaveButtonPrefix) ? "INPUT P" : _slaveButtonPrefix.Trim();
        PreviewText = $"{prefix}1, {prefix}2, {prefix}3...";
    }

    private async void SaveSettings()
    {
        try
        {
            // Save the prefix to configuration
            _configurationService.SetSlaveButtonPrefix(_slaveButtonPrefix);
            await _configurationService.SaveConfigurationAsync();
            
            // TODO: Show success message or notification
            System.Diagnostics.Debug.WriteLine("Settings saved successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save settings: {ex.Message}");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
