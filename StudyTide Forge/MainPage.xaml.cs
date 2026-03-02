using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Maui.Storage;

namespace StudyTideForge;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        blazorWebView.BlazorWebViewInitializing += OnBlazorWebViewInitializing;
    }

    private static void OnBlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs args)
    {
        var userDataFolder = Path.Combine(FileSystem.AppDataDirectory, "webview2");
        Directory.CreateDirectory(userDataFolder);
        args.UserDataFolder = userDataFolder;
    }
}
