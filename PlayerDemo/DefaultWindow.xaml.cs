using MahApps.Metro.Controls;
using PlayerDemo.ViewModels;
using System.Windows;

namespace PlayerDemo;

/// <summary>
/// DefaultWindow.xaml 的交互逻辑 MVVM 默认模式
/// </summary>
public partial class DefaultWindow : MetroWindow
{
    public DefaultViewModel mainViewModel
    {
        get; set;
    }
    public DefaultWindow()
    {
        InitializeComponent();
        mainViewModel = new DefaultViewModel(this);
        this.DataContext = mainViewModel;
        host.Overlay.SizeChanged += Overlay_SizeChanged;
    }

    private void Overlay_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
    {
        if (sender is Window Overlay)
        {
            mainViewModel.PlayerControlWidth = Overlay.ActualWidth * 0.8;
        }

    }
}
