﻿using System.Diagnostics;
using System.Reflection;
using System.Windows;
using static ArnoldVinkCode.AVFirewall;
using static LibraryShared.AppStartupCheck;

namespace KeyboardController
{
    public partial class App : Application
    {
        //Application Windows
        public static WindowMain vWindowMain = new WindowMain();
        public static WindowSettings vWindowSettings = new WindowSettings();

        //Application Startup
        protected override async void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Check the application update
                Application_UpdateCheck();

                //Check the application status
                await Application_LaunchCheck("Keyboard Controller", ProcessPriorityClass.High, false, false);

                //Allow application in firewall
                string appFilePath = Assembly.GetEntryAssembly().Location;
                Firewall_ExecutableAllow("Keyboard Controller", appFilePath, true);

                //Open the application window
                vWindowMain.Show();
            }
            catch { }
        }
    }
}