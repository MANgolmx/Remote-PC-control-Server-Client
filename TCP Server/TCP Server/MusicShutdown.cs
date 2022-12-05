using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Windows.Media.Control;

public class MusicShutdown
{
    private static GlobalSystemMediaTransportControlsSessionManager gsmtcsm;
    private static GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties;
    private static GlobalSystemMediaTransportControlsSessionMediaProperties oldMediaProperties;

    private static System.Timers.Timer MusicCheckTimer;
    private static int MusicCounter;

    public MusicShutdown() { }

    public async Task Initializing(int musicCounter)
    {
        Console.WriteLine("Initializing...");
        try
        {
            MusicCheckTimer = new System.Timers.Timer(200);
            MusicCheckTimer.AutoReset = true;
            MusicCheckTimer.Elapsed += CheckMusic;

            MusicCounter = musicCounter;

            gsmtcsm = await GetSystemMediaTransportControlsSessionManager();
            mediaProperties = await GetMediaProperties(gsmtcsm.GetCurrentSession());
            oldMediaProperties = mediaProperties;
            
            MusicCheckTimer.Start();
        } catch (Exception exc)
        {
            new ToastContentBuilder().AddText("Error occured: " + exc.Message).Show();

            Console.WriteLine("Error occured!");
            Console.WriteLine(exc.Message + "\n");
        }
        Console.WriteLine("Initializing finished!");
    }

    private static async void CheckMusic(Object source, ElapsedEventArgs e)
    {
        try
        {
            mediaProperties = await GetMediaProperties(gsmtcsm.GetCurrentSession());
            if (oldMediaProperties.Artist != mediaProperties.Artist && oldMediaProperties.Title != mediaProperties.Title)
            {
                MusicCounter--;
                oldMediaProperties = mediaProperties;
                if (MusicCounter == 0)
                {
                    Process.Start("Shutdown", "-s -t 1");
                    Environment.Exit(0);
                }
                Console.WriteLine("Song changed; Left - {0}", MusicCounter);
            }
        } catch(Exception exc)
        {
            new ToastContentBuilder().AddText("Error occured: " + exc.Message).Show();

            Console.WriteLine("Error occured!");
            Console.WriteLine(exc.Message + "\n");
        }
    }

    private static async Task<GlobalSystemMediaTransportControlsSessionManager> GetSystemMediaTransportControlsSessionManager() =>
        await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();

    private static async Task<GlobalSystemMediaTransportControlsSessionMediaProperties> GetMediaProperties(GlobalSystemMediaTransportControlsSession session) =>
        await session.TryGetMediaPropertiesAsync();
}
