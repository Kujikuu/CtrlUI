﻿using System.Diagnostics;
using static ArnoldVinkCode.AVFiles;

namespace LibraryShared
{
    public partial class AppStartupCheck
    {
        public static void Application_UpdateCheck()
        {
            try
            {
                Debug.WriteLine("Checking application update.");

                //Remove old unused files
                File_Delete("Resources/LibraryUsb.dll");
                File_Delete("Profiles/AppsBlacklistProcess.json");
                File_Delete("Profiles/AppsBlacklistShortcut.json");
                File_Delete("Profiles/AppsBlacklistShortcutUri.json");
                File_Delete("Profiles/AppsCloseLaunchers.json");
                File_Delete("Profiles/AppsCloseTools.json");
                File_Delete("Profiles/ControllersSupported.json");
                File_Delete("Profiles/DirectControllersIgnored.json");
                File_Delete("Profiles/FileLocations.json");
                File_Delete("Profiles/FpsBlacklistProcess.json");
                File_Delete("Profiles/FpsPositionProcess.json");
                File_Delete("Profiles/ShortcutLocations.json");
                File_Delete("Assets/Default/Custom.ttf");
                File_Delete("KeyboardController-Admin.exe");
                File_Delete("KeyboardController-Admin.exe.config");
                File_Delete("KeyboardController-Launcher.exe");
                File_Delete("KeyboardController-Launcher.exe.config");
                File_Delete("KeyboardController.exe");
                File_Delete("KeyboardController.exe.config");

                //Move old profiles
                File_Move("Profiles/CtrlApplications.json", "Profiles/User/CtrlApplications.json", true);
                File_Move("Profiles/DirectControllersProfile.json", "Profiles/User/DirectControllersProfile.json", true);
                File_Move("Profiles/DirectKeyboardTextList.json", "Profiles/User/DirectKeyboardTextList.json", true);
                File_Move("Profiles/FpsPositionProcessName.json", "Profiles/User/FpsPositionProcessName.json", true);

                //Rename old folder names
                Directory_Move("Assets/Roms", "Assets/User/Games", true);

                //Check - If updater has been updated
                File_Move("UpdaterNew.exe", "Updater.exe", true);
                File_Move("Resources/UpdaterReplace.exe", "Updater.exe", true);

                //Check - If updater failed to cleanup
                File_Delete("Resources/AppUpdate.zip");
            }
            catch { }
        }
    }
}