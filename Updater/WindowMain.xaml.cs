﻿using ArnoldVinkCode;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.ApiGitHub;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.ProcessWin32Functions;

namespace Updater
{
    public partial class WindowMain : Window
    {
        //Window Initialize
        public WindowMain() { InitializeComponent(); }

        //Window Initialized
        protected override async void OnSourceInitialized(EventArgs e)
        {
            try
            {
                //Check if previous update files exist
                File_Delete("Resources/UpdaterReplace.exe");
                File_Delete("Resources/AppUpdate.zip");

                //Check if CtrlUI is running and close it
                bool CtrlUIRunning = false;
                foreach (Process CloseProcess in Process.GetProcessesByName("CtrlUI"))
                {
                    CtrlUIRunning = true;
                    CloseProcess.Kill();
                }
                foreach (Process CloseProcess in Process.GetProcessesByName("CtrlUI-Admin"))
                {
                    CloseProcess.Kill();
                }

                //Check if DirectXInput is running and close it
                bool DirectXInputRunning = false;
                foreach (Process CloseProcess in Process.GetProcessesByName("DirectXInput"))
                {
                    DirectXInputRunning = true;
                    CloseProcess.Kill();
                }
                foreach (Process CloseProcess in Process.GetProcessesByName("DirectXInput-Admin"))
                {
                    CloseProcess.Kill();
                }

                //Check if Driver Installer is running and close it
                foreach (Process CloseProcess in Process.GetProcessesByName("DriverInstaller"))
                {
                    CloseProcess.Kill();
                }

                //Check if Fps Overlayer is running and close it
                bool FpsOverlayerRunning = false;
                foreach (Process CloseProcess in Process.GetProcessesByName("FpsOverlayer"))
                {
                    FpsOverlayerRunning = true;
                    CloseProcess.Kill();
                }
                foreach (Process CloseProcess in Process.GetProcessesByName("FpsOverlayer-Admin"))
                {
                    CloseProcess.Kill();
                }
                foreach (Process CloseProcess in Process.GetProcessesByName("FpsOverlayer-Launcher"))
                {
                    CloseProcess.Kill();
                }

                //Wait for applications to have closed
                await Task.Delay(1000);

                //Download application update from the website
                try
                {
                    ServicePointManager.SecurityProtocol |= SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    WebClient WebClient = new WebClient();
                    WebClient.Headers[HttpRequestHeader.UserAgent] = "Application Updater";
                    WebClient.DownloadProgressChanged += (object Object, DownloadProgressChangedEventArgs Args) =>
                    {
                        ProgressBarUpdate(Args.ProgressPercentage, false);
                        TextBlockUpdate("Downloading update file: " + Args.ProgressPercentage + "%");
                    };

                    Uri downloadUri = new Uri(ApiGitHub_GetDownloadPath("dumbie", "CtrlUI", "CtrlUI.zip"));
                    await WebClient.DownloadFileTaskAsync(downloadUri, "Resources/AppUpdate.zip");
                    Debug.WriteLine("Update file has been downloaded");
                }
                catch
                {
                    await Application_Exit("Failed to download update, closing in a bit.");
                    return;
                }

                //Delete the old drivers directory
                Directory_Delete("Resources/Drivers");

                //Extract the downloaded update archive
                try
                {
                    TextBlockUpdate("Updating the application to the latest version.");
                    using (ZipArchive ZipArchive = ZipFile.OpenRead("Resources/AppUpdate.zip"))
                    {
                        foreach (ZipArchiveEntry ZipFile in ZipArchive.Entries)
                        {
                            string ExtractPath = AVFunctions.StringReplaceFirst(ZipFile.FullName, "CtrlUI/", "", false);
                            if (!string.IsNullOrWhiteSpace(ExtractPath))
                            {
                                if (string.IsNullOrWhiteSpace(ZipFile.Name))
                                {
                                    Directory_Create(ExtractPath, false);
                                }
                                else
                                {
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("CtrlApplications.json".ToLower())) { Debug.WriteLine("Skipping: CtrlApplications.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("CtrlIgnoreProcessName.json".ToLower())) { Debug.WriteLine("Skipping: CtrlIgnoreProcessName.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("CtrlIgnoreLauncherName.json".ToLower())) { Debug.WriteLine("Skipping: CtrlIgnoreLauncherName.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("CtrlIgnoreShortcutName.json".ToLower())) { Debug.WriteLine("Skipping: CtrlIgnoreShortcutName.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("CtrlIgnoreShortcutUri.json".ToLower())) { Debug.WriteLine("Skipping: CtrlIgnoreShortcutUri.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("CtrlKeyboardExtensionName.json".ToLower())) { Debug.WriteLine("Skipping: CtrlKeyboardExtensionName.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("CtrlKeyboardProcessName.json".ToLower())) { Debug.WriteLine("Skipping: CtrlKeyboardProcessName.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("CtrlLocationsFile.json".ToLower())) { Debug.WriteLine("Skipping: CtrlLocationsFile.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("CtrlLocationsShortcut.json".ToLower())) { Debug.WriteLine("Skipping: CtrlLocationsShortcut.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("FpsPositionProcessName.json".ToLower())) { Debug.WriteLine("Skipping: FpsPositionProcessName.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("DirectControllersIgnoredUser.json".ToLower())) { Debug.WriteLine("Skipping: DirectControllersIgnoredUser.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("DirectControllersProfile.json".ToLower())) { Debug.WriteLine("Skipping: DirectControllersProfile.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("DirectKeypadMapping.json".ToLower())) { Debug.WriteLine("Skipping: DirectKeypadMapping.json"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("DirectKeyboardTextList.json".ToLower())) { Debug.WriteLine("Skipping: DirectKeyboardTextList.json"); continue; }

                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("Background.png".ToLower())) { Debug.WriteLine("Skipping: Background.png"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("BackgroundLive.mp4".ToLower())) { Debug.WriteLine("Skipping: BackgroundLive.mp4"); continue; }

                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("CtrlUI.exe.csettings".ToLower())) { Debug.WriteLine("Skipping: CtrlUI.exe.csettings"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("DirectXInput.exe.csettings".ToLower())) { Debug.WriteLine("Skipping: DirectXInput.exe.csettings"); continue; }
                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("FpsOverlayer.exe.csettings".ToLower())) { Debug.WriteLine("Skipping: FpsOverlayer.exe.csettings"); continue; }

                                    if (File.Exists(ExtractPath) && ExtractPath.ToLower().EndsWith("Updater.exe".ToLower()))
                                    {
                                        Debug.WriteLine("Renaming: Updater.exe");
                                        ExtractPath = ExtractPath.Replace("Updater.exe", "Resources/UpdaterReplace.exe");
                                    }

                                    ZipFile.ExtractToFile(ExtractPath, true);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    await Application_Exit("Failed to extract update, closing in a bit.");
                    return;
                }

                //Start CtrlUI after the update has completed.
                if (CtrlUIRunning)
                {
                    TextBlockUpdate("Running the updated version of the application.");
                    await ProcessLauncherWin32Async("CtrlUI-Admin.exe", "", "", true, false);
                }

                //Start DirectXInput after the update has completed.
                if (DirectXInputRunning)
                {
                    TextBlockUpdate("Running the updated version of the application.");
                    await ProcessLauncherWin32Async("DirectXInput-Admin.exe", "", "", true, false);
                }

                //Start FpsOverlayer after the update has completed.
                if (FpsOverlayerRunning)
                {
                    TextBlockUpdate("Running the updated version of the application.");
                    await ProcessLauncherWin32Async("FpsOverlayer-Admin.exe", "", "", true, false);
                }

                //Close the application
                await Application_Exit("Application has been updated, closing in a bit.");
            }
            catch { }
        }

        //Update the textblock
        public void TextBlockUpdate(string Text)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    textblock_Status.Text = Text;
                });
            }
            catch { }
        }

        //Update the progressbar
        public void ProgressBarUpdate(double Progress, bool Indeterminate)
        {
            try
            {
                AVActions.ActionDispatcherInvoke(delegate
                {
                    progressbar_Status.IsIndeterminate = Indeterminate;
                    progressbar_Status.Value = Progress;
                });
            }
            catch { }
        }

        //Application Close Handler
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
            }
            catch { }
        }

        //Close the application
        async Task Application_Exit(string ExitMessage)
        {
            try
            {
                Debug.WriteLine("Exiting application.");
                AVActions.ActionDispatcherInvoke(delegate
                {
                    this.Opacity = 0.80;
                    this.IsEnabled = false;
                });

                //Delete the update installation zip file
                File_Delete("Resources/AppUpdate.zip");

                //Set the exit reason text message
                TextBlockUpdate(ExitMessage);
                ProgressBarUpdate(100, false);

                //Close the application after x seconds
                await Task.Delay(2000);
                Environment.Exit(0);
            }
            catch { }
        }
    }
}