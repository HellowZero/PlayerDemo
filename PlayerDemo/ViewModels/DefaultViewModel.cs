using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlyleafLib;
using FlyleafLib.Controls.WPF;
using FlyleafLib.MediaPlayer;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace PlayerDemo.ViewModels;

public partial class DefaultViewModel : ObservableObject
{
    public DefaultViewModel(Window window)
    {
        CurrentWindow = window;

        CurrentWindow.Loaded += CurrentWindow_Loaded;
    }

    private Player player;

    public Player Player
    {
        get => player;
        set => SetProperty(ref player, value);
    }

    private Config config;

    public Config PlayerConfig
    {
        get => config;
        set => SetProperty(ref config, value);
    }

    private string uriString;

    public string UriString
    {
        get => uriString;
        set => SetProperty(ref uriString, value);
    }




    public Window CurrentWindow
    {
        get;
    }

    static bool runOnce;
    Config playerConfig;
    bool ReversePlaybackChecked;

    [RelayCommand]
    private void PlayAction()
    {

        OpenFileDialog openFileDialog = new OpenFileDialog();
        var result = openFileDialog.ShowDialog();
        if (result == DialogResult.OK)
        {
            var Path = openFileDialog.FileName;
            Player.OpenAsync(Path);
        }

    }
    [RelayCommand]
    private void FullScreenWin()
    {
        bool IsFullScreen = Player.Host.Player_GetFullScreen();
        Player.Host.Player_SetFullScreen(!IsFullScreen);
    }
    private void CurrentWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (Engine.IsLoaded)
        {
            LoadPlayer();
        }
        else
        {
            Engine.Loaded += (o, e) =>
            {
                LoadPlayer();
            };
        }

        if (runOnce)
            return;

        runOnce = true;
#if RELEASE
            // Save Player's Config (First Run)
            // Ensures that the Control's handle has been created and the renderer has been fully initialized (so we can save also the filters parsed by the library)
            if (!playerConfig.Loaded)
            {
                try
                {
                    Utils.AddFirewallRule();
                    playerConfig.Save("Flyleaf.Config.json");
                } catch { }
            }

            // Stops Logging (First Run)
            if (!Engine.Config.Loaded)
            {
                Engine.Config.LogOutput      = null;
                Engine.Config.LogLevel       = LogLevel.Quiet;
                //Engine.Config.FFmpegDevices  = false;

                try { Engine.Config.Save("Flyleaf.Engine.json"); } catch { }
            }
#endif
    }
    private void LoadPlayer()
    {
#if RELEASE
            // Load Player's Config
            if (File.Exists("Flyleaf.Config.json"))
                try { playerConfig = Config.Load("Flyleaf.Config.json"); } catch { playerConfig = DefaultConfig(); }
            else
                playerConfig = DefaultConfig();
#else
        var config = DefaultConfig();
#endif

        Player = new Player(config);

        // If the user requests reverse playback allocate more frames once
        Player.PropertyChanged += (o, e) =>
        {
            if (e.PropertyName == "ReversePlayback")
            {
                if (playerConfig.Decoder.MaxVideoFrames < 80)
                    playerConfig.Decoder.MaxVideoFrames = 80;


            }
            else if (e.PropertyName == nameof(Player.Status) && Player.Activity.Mode == ActivityMode.Idle)
                Player.Activity.ForceActive();

        };
    }

    private Config DefaultConfig()
    {
        Config config = new Config();
        config.Audio.FiltersEnabled = true;         // To allow embedded atempo filter for speed
        config.Video.GPUAdapter = "";           // Set it empty so it will include it when we save it
        config.Subtitles.SearchLocal = true;
        return config;
    }





    #region OSD Msg
    const int ACTIVITY_TIMEOUT = 3500;
    CancellationTokenSource cancelMsgToken = new();
    public string Msg
    {
        get => msg; set
        {
            cancelMsgToken.Cancel(); msg = value;
            OnPropertyChanged(Msg);
            cancelMsgToken = new();
            Task.Run(FadeOutMsg, cancelMsgToken.Token);
        }
    }
    string msg;
    private async Task FadeOutMsg()
    {
        await Task.Delay(ACTIVITY_TIMEOUT, cancelMsgToken.Token);
        Utils.UIInvoke(() =>
        {
            msg = "";
            OnPropertyChanged(Msg);
        });
    }
    #endregion
}
