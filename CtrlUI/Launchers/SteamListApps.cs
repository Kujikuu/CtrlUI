﻿using Microsoft.Win32;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        public static string[] vSteamIdBlacklist = { "218", "228980" };

        string SteamInstallPath()
        {
            try
            {
                //Open the Windows registry
                using (RegistryKey registryKeyCurrentUser = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                {
                    //Search for Steam install directory
                    using (RegistryKey RegKeySteam = registryKeyCurrentUser.OpenSubKey("Software\\Valve\\Steam"))
                    {
                        if (RegKeySteam != null)
                        {
                            string RegKeyExePath = RegKeySteam.GetValue("SteamExe").ToString();
                            if (File.Exists(RegKeyExePath))
                            {
                                return Path.GetDirectoryName(RegKeyExePath);
                            }
                        }
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        List<string> SteamLibraryPaths()
        {
            List<string> libraryPaths = new List<string>();
            try
            {
                //Get steam main installation path
                string steamInstallPath = SteamInstallPath();
                if (string.IsNullOrWhiteSpace(steamInstallPath))
                {
                    Debug.Write("Steam installation not found.");
                    return libraryPaths;
                }

                //Set steam library folders path
                string steamAppsPath = Path.Combine(steamInstallPath, "steamapps\\libraryfolders.vdf");

                //Parse steam library folder file
                KeyValue keyValue = new KeyValue();
                using (FileStream fileStream = new FileStream(steamAppsPath, FileMode.Open, FileAccess.Read))
                {
                    keyValue.ReadAsText(fileStream);
                }

                //Add steam installation paths
                libraryPaths.Add(steamInstallPath);
                foreach (KeyValue child in keyValue.Children)
                {
                    try
                    {
                        if (child.Value.Contains("\\") && Directory.Exists(child.Value))
                        {
                            libraryPaths.Add(child.Value);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return libraryPaths;
        }

        async Task SteamScanAddLibrary()
        {
            try
            {
                //Get steam library paths
                List<string> libraryPaths = SteamLibraryPaths();

                //Get steam main path
                string steamMainPath = libraryPaths.FirstOrDefault();

                //Get launcher icon image
                BitmapImage launcherImage = FileToBitmapImage(new string[] { "Steam" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 10, 0);

                foreach (string path in libraryPaths)
                {
                    try
                    {
                        string steamAppsPath = Path.Combine(path, "steamapps");
                        //Debug.WriteLine("Scanning steam library: " + steamAppsPath);
                        foreach (string manifestPath in Directory.GetFiles(steamAppsPath, "appmanifest*.acf"))
                        {
                            await SteamAddApplication(manifestPath, steamMainPath, launcherImage);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed scanning steam library: " + ex.Message);
                    }
                }
            }
            catch { }
        }

        async Task SteamAddApplication(string appmanifestPath, string steamMainPath, BitmapImage launcherImage)
        {
            try
            {
                KeyValue keyValue = new KeyValue();
                keyValue.ReadFileAsText(appmanifestPath);

                //Get application id
                string appId = keyValue["appID"].Value;

                //Get launch argument
                string runCommand = "steam://rungameid/" + appId;
                vLauncherAppAvailableCheck.Add(runCommand);

                //Check if application is already added
                DataBindApp launcherExistCheck = List_Launchers.Where(x => x.PathExe.ToLower() == runCommand.ToLower()).FirstOrDefault();
                if (launcherExistCheck != null)
                {
                    //Debug.WriteLine("Steam app already in list: " + appId);
                    return;
                }

                //Check if application id is in blacklist
                if (vSteamIdBlacklist.Contains(appId))
                {
                    Debug.WriteLine("Steam id is blacklisted: " + appId);
                    return;
                }

                //Get application name
                string appName = keyValue["name"].Value;
                if (string.IsNullOrWhiteSpace(appName) || appName.Contains("appid"))
                {
                    appName = keyValue["installDir"].Value;
                }

                //Get application image
                string libraryImageName = steamMainPath + "\\appcache\\librarycache\\" + appId + "_library_600x900.jpg";
                string logoImageName = steamMainPath + "\\appcache\\librarycache\\" + appId + "_logo.png";
                BitmapImage iconBitmapImage = FileToBitmapImage(new string[] { libraryImageName, logoImageName, "Steam" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 90, 0);

                //Add the application to the list
                DataBindApp dataBindApp = new DataBindApp()
                {
                    Category = AppCategory.Launcher,
                    Launcher = AppLauncher.Steam,
                    Name = appName,
                    ImageBitmap = iconBitmapImage,
                    PathExe = runCommand,
                    StatusLauncher = launcherImage
                };
                await ListBoxAddItem(lb_Launchers, List_Launchers, dataBindApp, false, false);
                //Debug.WriteLine("Added steam app: " + appId + "/" + appName);
            }
            catch
            {
                Debug.WriteLine("Failed adding steam app: " + appmanifestPath);
            }
        }
    }
}