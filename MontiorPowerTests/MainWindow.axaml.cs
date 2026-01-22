using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MontiorPowerTests;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void BroadcastMessageButtonOnClick(object? sender, RoutedEventArgs e)
    {
        NativeMethods.TurnOffMonitor(MessageType.BroadcastSystemMessage);
    }

    private void SendMessageButtonOnClick(object? sender, RoutedEventArgs e)
    {
        NativeMethods.TurnOffMonitor(MessageType.SendMessageToTempWindow);
    }

    private void SendMessageToTopLevelWindowsButtonOnClick(object? sender, RoutedEventArgs e)
    {
        NativeMethods.TurnOffMonitor(MessageType.SendMessageToTopLevelWindows);
    }
    
    private void PostMessageButtonOnClick(object? sender, RoutedEventArgs e)
    {
        NativeMethods.TurnOffMonitor(MessageType.PostMessage);
    }
    private void PostThreadMessageButtonOnClick(object? sender, RoutedEventArgs e)
    {
        NativeMethods.TurnOffMonitor(MessageType.PostThreadMessage);
    }
    private void DefaultWindowProcButtonOnClick(object? sender, RoutedEventArgs e)
    {
        NativeMethods.TurnOffMonitor(MessageType.DefWindowProcFromDesktopWindow);
    }
}