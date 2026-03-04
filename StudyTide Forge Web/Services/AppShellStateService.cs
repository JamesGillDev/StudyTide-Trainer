namespace StudyTideForge.Services;

public sealed class AppShellStateService
{
    public const string AlphaWavesDefaultPath = @"C:\MSSA Code-github\StudyTide Trainer\StudyTide Forge\Resources\Audio\8_Hour_Alpha_Waves.mp3";

    public event Action? Changed;

    public bool ShowLaunchTipBanner { get; private set; } = true;

    public bool ShowMediaPlayer { get; private set; }

    public bool AlphaWavesEnabled { get; private set; }

    public double MediaVolume { get; private set; } = 0.45;

    public int AlphaWavesActivationStamp { get; private set; }

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
        Changed?.Invoke();
    }
}
