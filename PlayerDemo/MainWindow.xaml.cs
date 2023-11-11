using System.Windows;
using FlyleafLib.Controls.WPF;
using MahApps.Metro.Controls;

namespace PlayerDemo;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainViewModel mainViewModel
    {
        get; set;
    }
    public MainWindow()
    {
        mainViewModel = new MainViewModel(this);
        InitializeComponent();
        
        this.DataContext = mainViewModel;
      
    }

}
