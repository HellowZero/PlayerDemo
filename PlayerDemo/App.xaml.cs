using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FlyleafLib;

namespace PlayerDemo;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // Ensures that we have enough worker threads to avoid the UI from freezing or not updating on time
        ThreadPool.GetMinThreads(out int workers, out int ports);
        ThreadPool.SetMinThreads(workers + 6, ports + 6);
        EngineConfig engineConfig;
        engineConfig = DefaultEngineConfig();
        Engine.StartAsync(engineConfig);
    }
    private EngineConfig DefaultEngineConfig()
    {
        EngineConfig engineConfig = new EngineConfig();

        engineConfig.PluginsPath = ":Plugins";
        engineConfig.FFmpegPath = ":FFmpeg";
        engineConfig.FFmpegHLSLiveSeek
                                    = true;
        engineConfig.UIRefresh = true;
        engineConfig.FFmpegDevices = true;

#if RELEASE
            engineConfig.LogOutput      = "Flyleaf.FirstRun.log";
            engineConfig.LogLevel       = LogLevel.Debug;
#else
        engineConfig.LogOutput = ":debug";
        engineConfig.LogLevel = LogLevel.Debug;
        engineConfig.FFmpegLogLevel = FFmpegLogLevel.Warning;
#endif

        return engineConfig;
    }
}
