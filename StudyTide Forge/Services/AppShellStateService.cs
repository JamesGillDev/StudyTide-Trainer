using System.Text.Json;

namespace StudyTideForge.Services;

public sealed class AppShellStateService
{
    public const string AlphaWavesDefaultPath = "/audio/8_Hour_Alpha_Waves.wav";
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly string _settingsPath = ResolveSettingsPath();

    public event Action? Changed;

    public bool ShowLaunchTipBanner { get; private set; } = true;

    public bool ShowMediaPlayer { get; private set; }

    public bool AlphaWavesEnabled { get; private set; }

    public double MediaVolume { get; private set; } = 0.45;

    public int AlphaWavesActivationStamp { get; private set; }

    public AppShellStateService()
    {
        LoadSettings();
    }

    public void SetShowLaunchTipBanner(bool enabled)
    {
        if (ShowLaunchTipBanner == enabled)
        {
            return;
        }

        ShowLaunchTipBanner = enabled;
        NotifyChanged();
    }

    public void SetShowMediaPlayer(bool enabled)
    {
        if (ShowMediaPlayer == enabled)
        {
            return;
        }

        ShowMediaPlayer = enabled;
        NotifyChanged();
    }

    public void SetAlphaWavesEnabled(bool enabled)
    {
        if (AlphaWavesEnabled == enabled)
        {
            return;
        }

        AlphaWavesEnabled = enabled;
        if (enabled)
        {
            AlphaWavesActivationStamp++;
            ShowMediaPlayer = true;
        }

        NotifyChanged();
    }

    public void SetMediaVolume(double volume)
    {
        var bounded = Math.Clamp(volume, 0, 1);
        if (Math.Abs(MediaVolume - bounded) < 0.0001)
        {
            return;
        }

        MediaVolume = bounded;
        NotifyChanged();
    }

    public void NotifyChanged()
    {
        SaveSettings();
        Changed?.Invoke();
    }

    private void LoadSettings()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                return;
            }

            var json = File.ReadAllText(_settingsPath);
            var settings = JsonSerializer.Deserialize<PersistedAppShellSettings>(json, JsonOptions);
            if (settings is null)
            {
                return;
            }

            ShowLaunchTipBanner = settings.ShowLaunchTipBanner;
            ShowMediaPlayer = settings.ShowMediaPlayer;
            AlphaWavesEnabled = settings.AlphaWavesEnabled;
            MediaVolume = Math.Clamp(settings.MediaVolume, 0, 1);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"[StudyTide] Unable to load shell settings. {exception.Message}");
        }
    }

    private void SaveSettings()
    {
        try
        {
            var directory = Path.GetDirectoryName(_settingsPath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var payload = new PersistedAppShellSettings
            {
                ShowLaunchTipBanner = ShowLaunchTipBanner,
                ShowMediaPlayer = ShowMediaPlayer,
                AlphaWavesEnabled = AlphaWavesEnabled,
                MediaVolume = MediaVolume
            };

            var json = JsonSerializer.Serialize(payload, JsonOptions);
            File.WriteAllText(_settingsPath, json);
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"[StudyTide] Unable to save shell settings. {exception.Message}");
        }
    }

    private static string ResolveSettingsPath()
    {
        var root = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StudyTideForge");

        return Path.Combine(root, "app-shell-settings.json");
    }

    private sealed class PersistedAppShellSettings
    {
        public bool ShowLaunchTipBanner { get; init; } = true;

        public bool ShowMediaPlayer { get; init; }

        public bool AlphaWavesEnabled { get; init; }

        public double MediaVolume { get; init; } = 0.45;
    }
}
