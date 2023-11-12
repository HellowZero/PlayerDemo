using MahApps.Metro.Controls;

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

    }
}
