﻿using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.ApplicationModel;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVSearch;
using static ArnoldVinkCode.AVUwpAppx;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Load application image
        public BitmapImage Image_Application_Load(DataBindApp dataBindApp, int imageSize)
        {
            BitmapImage applicationImage = null;
            try
            {
                if (dataBindApp.Type == ProcessType.UWP || dataBindApp.Type == ProcessType.Win32Store)
                {
                    string imageFileName = AVFiles.FileNameReplaceInvalidChars(dataBindApp.Name, string.Empty);
                    string imageFileExeName = Path.GetFileNameWithoutExtension(dataBindApp.NameExe);
                    string imageSquareLargestLogoPath = string.Empty;
                    string imageWideLargestLogoPath = string.Empty;

                    //Get application image information
                    try
                    {
                        Package appPackage = GetUwpAppPackageByAppUserModelId(dataBindApp.AppUserModelId);
                        AppxDetails appxDetails = GetUwpAppxDetailsByUwpAppPackage(appPackage);
                        imageFileExeName = Path.GetFileNameWithoutExtension(appxDetails.ExecutableAliasName);
                        imageSquareLargestLogoPath = appxDetails.SquareLargestLogoPath;
                        imageWideLargestLogoPath = appxDetails.WideLargestLogoPath;
                    }
                    catch { }

                    //Set application bitmap image
                    applicationImage = FileToBitmapImage(new string[] { imageFileName, imageFileExeName, imageSquareLargestLogoPath, imageWideLargestLogoPath }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, imageSize, 0);
                }
                else
                {
                    //Get application image information
                    string imageFileName = AVFiles.FileNameReplaceInvalidChars(dataBindApp.Name, string.Empty);
                    string imageFileExeName = Path.GetFileNameWithoutExtension(dataBindApp.NameExe);
                    string imageFileExePath = dataBindApp.PathExe;

                    //Set application bitmap image
                    applicationImage = FileToBitmapImage(new string[] { imageFileName, imageFileExeName, imageFileExePath }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
                }

                Debug.WriteLine("Loaded application image: " + applicationImage);
                return applicationImage;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to load application image: " + ex.Message);
                return applicationImage;
            }
        }

        //Remove application image
        public void Image_Application_Remove(DataBindApp dataBindApp)
        {
            try
            {
                //Set application and executable name
                string imageFileName = AVFiles.FileNameReplaceInvalidChars(dataBindApp.Name, string.Empty);
                string imageFileExeName = Path.GetFileNameWithoutExtension(dataBindApp.PathExe);

                //Search application image files
                string[] foundImages = Search_Files(new string[] { imageFileName, imageFileExeName }, vImageSourceFoldersUser, false);

                //Remove application image files
                foreach (string foundImage in foundImages)
                {
                    File_Delete(foundImage);
                }

                Debug.WriteLine("Application image removed: " + imageFileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to remove application image: " + ex.Message);
            }
        }

        //Reset application image
        public async Task Image_Application_Reset(DataBindApp dataBindApp)
        {
            try
            {
                //Set application and executable name
                string imageFileName = string.Empty;
                string imageFileExeName = string.Empty;
                string imageFileExePath = string.Empty;
                if (dataBindApp != null)
                {
                    imageFileName = AVFiles.FileNameReplaceInvalidChars(dataBindApp.Name, string.Empty);
                    imageFileExeName = Path.GetFileNameWithoutExtension(dataBindApp.NameExe);
                    imageFileExePath = dataBindApp.PathExe;
                }
                else
                {
                    imageFileName = AVFiles.FileNameReplaceInvalidChars(tb_AddAppName.Text, string.Empty);
                    imageFileExeName = Path.GetFileNameWithoutExtension(tb_AddAppPathExe.Text);
                    imageFileExePath = tb_AddAppPathExe.Text;
                }

                //Search application image files
                string[] foundImages = Search_Files(new string[] { imageFileName, imageFileExeName }, vImageSourceFoldersUser, false);

                //Remove application image files
                foreach (string foundImage in foundImages)
                {
                    File_Delete(foundImage);
                }

                //Reload the application image
                BitmapImage applicationImage = null;
                if (dataBindApp != null)
                {
                    applicationImage = dataBindApp.ImageBitmap = Image_Application_Load(dataBindApp, vImageLoadSize);
                }
                else
                {
                    applicationImage = FileToBitmapImage(new string[] { imageFileName, imageFileExeName, imageFileExePath }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, vImageLoadSize, 0);
                }
                dataBindApp.ImageBitmap = applicationImage;
                img_AddAppLogo.Source = applicationImage;

                await Notification_Send_Status("Restart", "Application image reset");
                Debug.WriteLine("Application image reset: " + imageFileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to reset application image: " + ex.Message);
            }
        }
    }
}