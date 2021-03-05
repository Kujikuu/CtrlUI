﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.ProcessUwpFunctions;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Settings;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Launch an UWP or Win32Store application from databindapp
        async Task<bool> PrepareProcessLauncherUwpAndWin32StoreAsync(DataBindApp dataBindApp, bool silent, bool allowMinimize, bool launchKeyboard)
        {
            bool appLaunched = false;
            try
            {
                //Launch the application
                appLaunched = await PrepareProcessLauncherUwpAndWin32StoreAsync(dataBindApp.Name, dataBindApp.PathExe, dataBindApp.Argument, silent, allowMinimize, launchKeyboard);

                //Update last launch date
                if (appLaunched)
                {
                    dataBindApp.LastLaunch = DateTime.Now.ToString(vAppCultureInfo);
                    //Debug.WriteLine("Updated last launch date: " + dataBindApp.LastLaunch);
                    JsonSaveApplications();
                }
            }
            catch { }
            return appLaunched;
        }

        //Launch an UWP or Win32Store application manually
        async Task<bool> PrepareProcessLauncherUwpAndWin32StoreAsync(string appTitle, string pathExe, string argument, bool silent, bool allowMinimize, bool launchKeyboard)
        {
            try
            {
                //Check if the application exists
                if (UwpGetAppPackageByAppUserModelId(pathExe) == null)
                {
                    await Notification_Send_Status("Close", "Application not found");
                    Debug.WriteLine("Launch application not found.");
                    return false;
                }

                //Show launching message
                if (!silent)
                {
                    await Notification_Send_Status("AppLaunch", "Launching " + appTitle);
                    //Debug.WriteLine("Launching UWP or Win32Store: " + appTitle + "/" + pathExe);
                }

                //Minimize the CtrlUI window
                if (allowMinimize && Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "MinimizeAppOnShow"))) { await AppMinimize(true); }

                //Launch the UWP or Win32Store application
                Process launchProcess = await ProcessLauncherUwpAndWin32StoreAsync(pathExe, argument);
                if (launchProcess == null)
                {
                    //Show failed launch messagebox
                    await LaunchProcessFailed();
                    return false;
                }

                //Launch the keyboard controller
                if (launchKeyboard)
                {
                    await KeyboardControllerHideShow(true);
                }

                return true;
            }
            catch
            {
                //Show failed launch messagebox
                await LaunchProcessFailed();
                return false;
            }
        }
    }
}