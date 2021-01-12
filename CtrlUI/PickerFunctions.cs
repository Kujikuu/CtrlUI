﻿using ArnoldVinkCode;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVFiles;
using static ArnoldVinkCode.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;
using static LibraryShared.Settings;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show the File Picker Popup Task
        async Task Popup_Show_FilePicker(string targetPath, int targetIndex, bool storeIndex, FrameworkElement previousFocus)
        {
            try
            {
                async Task TaskAction()
                {
                    try
                    {
                        await Popup_Show_FilePicker_Task(targetPath, targetIndex, storeIndex, previousFocus);
                    }
                    catch { }
                }
                await AVActions.TaskStartReturn(TaskAction);
            }
            catch { }
        }

        //Show the File Picker Popup
        async Task Popup_Show_FilePicker_Task(string targetPath, int targetIndex, bool storeIndex, FrameworkElement previousFocus)
        {
            try
            {
                //Check if the popup is already open
                if (!vFilePickerOpen)
                {
                    //Play the popup opening sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "PopupOpen", false);

                    //Save the previous focus element
                    Popup_PreviousElementFocus_Save(vFilePickerElementFocus, previousFocus);
                }

                //Reset file picker variables
                vFilePickerCompleted = false;
                vFilePickerCancelled = false;
                vFilePickerResult = null;
                vFilePickerOpen = true;

                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Disable the file picker list
                    lb_FilePicker.IsEnabled = false;
                    gif_FilePicker_Loading.Show();

                    //Set file picker header texts
                    grid_Popup_FilePicker_txt_Title.Text = vFilePickerTitle;
                    grid_Popup_FilePicker_txt_Description.Text = vFilePickerDescription;

                    //Change the list picker item style
                    if (vFilePickerShowRoms)
                    {
                        lb_FilePicker.Style = Application.Current.Resources["ListBoxWrapPanel"] as Style;
                        lb_FilePicker.ItemTemplate = Application.Current.Resources["ListBoxItemRom"] as DataTemplate;
                        grid_Popup_Filepicker_Row1.HorizontalAlignment = HorizontalAlignment.Center;
                    }
                    else
                    {
                        lb_FilePicker.Style = Application.Current.Resources["ListBoxVertical"] as Style;
                        lb_FilePicker.ItemTemplate = Application.Current.Resources["ListBoxItemFile"] as DataTemplate;
                        grid_Popup_Filepicker_Row1.HorizontalAlignment = HorizontalAlignment.Stretch;
                    }
                });

                //Show the popup
                Popup_Show_Element(grid_Popup_FilePicker);

                //Update the current path
                vFilePickerCurrentPath = targetPath;

                AVActions.ActionDispatcherInvoke(delegate
                {
                    //Update the current index
                    if (storeIndex)
                    {
                        int selectedIndex = lb_FilePicker.SelectedIndex;
                        Debug.WriteLine("Adding navigation history index: " + selectedIndex);
                        vFilePickerNavigateIndexes.Add(selectedIndex);
                    }

                    //Clear the current file picker list
                    List_FilePicker.Clear();
                });

                //Get and list all the disk drives
                if (targetPath == "PC")
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        //Enable or disable selection button in the list
                        grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;

                        //Enable or disable file and folder availability
                        grid_Popup_FilePicker_textblock_NoFilesAvailable.Visibility = Visibility.Collapsed;

                        //Enable or disable the side navigate buttons
                        grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Visible;
                        grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Collapsed;
                        grid_Popup_FilePicker_button_ControllerStart.Visibility = Visibility.Collapsed;

                        //Enable or disable the copy paste status
                        if (vClipboardFiles.Any())
                        {
                            grid_Popup_FilePicker_textblock_ClipboardStatus.Visibility = Visibility.Visible;
                        }

                        //Enable or disable the current path
                        grid_Popup_FilePicker_textblock_CurrentPath.Visibility = Visibility.Collapsed;
                    });

                    //Load folder images
                    BitmapImage imageFolder = FileToBitmapImage(new string[] { "Assets/Default/Icons/Folder.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    BitmapImage imageFolderDisc = FileToBitmapImage(new string[] { "Assets/Default/Icons/FolderDisc.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    BitmapImage imageFolderNetwork = FileToBitmapImage(new string[] { "Assets/Default/Icons/FolderNetwork.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    BitmapImage imageFolderPrevious = FileToBitmapImage(new string[] { "Assets/Default/Icons/Restart.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    BitmapImage imageFolderDocuments = FileToBitmapImage(new string[] { "Assets/Default/Icons/Copy.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    BitmapImage imageFolderDesktop = FileToBitmapImage(new string[] { "Assets/Default/Icons/Desktop.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    BitmapImage imageFolderDownload = FileToBitmapImage(new string[] { "Assets/Default/Icons/Download.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    BitmapImage imageFolderPictures = FileToBitmapImage(new string[] { "Assets/Default/Icons/Background.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    BitmapImage imageFolderVideos = FileToBitmapImage(new string[] { "Assets/Default/Icons/BackgroundVideo.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                    BitmapImage imageFolderMusic = FileToBitmapImage(new string[] { "Assets/Default/Icons/Music.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);

                    //Add launch without a file option
                    if (vFilePickerShowNoFile)
                    {
                        string fileDescription = "Launch application without a file";
                        BitmapImage fileImage = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppLaunch.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        DataBindFile dataBindFileWithoutFile = new DataBindFile() { FileType = FileType.FilePre, Name = fileDescription, Description = fileDescription + ".", ImageBitmap = fileImage, PathFile = string.Empty };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileWithoutFile, false, false);
                    }

                    //Check and add the previous path
                    if (!string.IsNullOrWhiteSpace(vFilePickerPreviousPath))
                    {
                        DataBindFile dataBindFilePreviousPath = new DataBindFile() { FileType = FileType.FolderPre, Name = "Previous", NameSub = "(" + vFilePickerPreviousPath + ")", ImageBitmap = imageFolderPrevious, PathFile = vFilePickerPreviousPath };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFilePreviousPath, false, false);
                    }

                    //Add special folders
                    DataBindFile dataBindFileDesktop = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Desktop", ImageBitmap = imageFolderDesktop, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDesktop, false, false);
                    DataBindFile dataBindFileDocuments = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Documents", ImageBitmap = imageFolderDocuments, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDocuments, false, false);

                    try
                    {
                        string downloadsPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", string.Empty).ToString();
                        if (!string.IsNullOrWhiteSpace(downloadsPath) && Directory.Exists(downloadsPath))
                        {
                            DataBindFile dataBindFileDownloads = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Downloads", ImageBitmap = imageFolderDownload, PathFile = downloadsPath };
                            await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDownloads, false, false);
                        }
                    }
                    catch { }

                    DataBindFile dataBindFileMusic = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Music", ImageBitmap = imageFolderMusic, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileMusic, false, false);
                    DataBindFile dataBindFilePictures = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Pictures", ImageBitmap = imageFolderPictures, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFilePictures, false, false);
                    DataBindFile dataBindFileVideos = new DataBindFile() { FileType = FileType.FolderPre, Name = "My Videos", ImageBitmap = imageFolderVideos, PathFile = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) };
                    await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileVideos, false, false);

                    //Add all disk drives that are connected
                    DriveInfo[] diskDrives = DriveInfo.GetDrives();
                    foreach (DriveInfo disk in diskDrives)
                    {
                        try
                        {
                            //Skip network drive depending on the setting
                            if (disk.DriveType == DriveType.Network && Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "HideNetworkDrives")))
                            {
                                continue;
                            }

                            //Check if the disk is currently connected
                            if (disk.IsReady)
                            {
                                //Get the current disk size
                                string freeSpace = AVFunctions.ConvertBytesSizeToString(disk.TotalFreeSpace);
                                string usedSpace = AVFunctions.ConvertBytesSizeToString(disk.TotalSize);
                                string diskSpace = freeSpace + "/" + usedSpace;

                                DataBindFile dataBindFileDisk = new DataBindFile() { FileType = FileType.Folder, Name = disk.Name, NameSub = disk.VolumeLabel, NameDetail = diskSpace, PathFile = disk.Name };
                                if (disk.DriveType == DriveType.CDRom)
                                {
                                    dataBindFileDisk.FileType = FileType.FolderDisc;
                                    dataBindFileDisk.ImageBitmap = imageFolderDisc;
                                }
                                else if (disk.DriveType == DriveType.Network)
                                {
                                    dataBindFileDisk.ImageBitmap = imageFolderNetwork;
                                }
                                else
                                {
                                    dataBindFileDisk.ImageBitmap = imageFolder;
                                }
                                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileDisk, false, false);
                            }
                        }
                        catch { }
                    }

                    //Add Json file locations
                    foreach (ProfileShared Locations in vCtrlLocationsFile)
                    {
                        try
                        {
                            if (Directory.Exists(Locations.String2))
                            {
                                //Check if the location is a root folder
                                FileType locationType = FileType.FolderPre;
                                DirectoryInfo locationInfo = new DirectoryInfo(Locations.String2);
                                if (locationInfo.Parent == null)
                                {
                                    locationType = FileType.Folder;
                                }

                                //Get the current disk size
                                string diskSpace = string.Empty;
                                DriveType disktype = DriveType.Unknown;
                                try
                                {
                                    DriveInfo driveInfo = new DriveInfo(Locations.String2);
                                    disktype = driveInfo.DriveType;
                                    string freeSpace = AVFunctions.ConvertBytesSizeToString(driveInfo.TotalFreeSpace);
                                    string usedSpace = AVFunctions.ConvertBytesSizeToString(driveInfo.TotalSize);
                                    diskSpace = freeSpace + "/" + usedSpace;
                                }
                                catch { }

                                DataBindFile dataBindFileLocation = new DataBindFile() { FileType = locationType, Name = Locations.String2, NameSub = Locations.String1, NameDetail = diskSpace, PathFile = Locations.String2 };
                                if (disktype == DriveType.CDRom)
                                {
                                    dataBindFileLocation.FileType = FileType.FolderDisc;
                                    dataBindFileLocation.ImageBitmap = imageFolderDisc;
                                }
                                else if (disktype == DriveType.Network)
                                {
                                    dataBindFileLocation.ImageBitmap = imageFolderNetwork;
                                }
                                else
                                {
                                    dataBindFileLocation.ImageBitmap = imageFolder;
                                }
                                await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileLocation, false, false);
                            }
                        }
                        catch { }
                    }
                }
                //Get and list all the UWP applications
                else if (targetPath == "UWP")
                {
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        //Enable or disable selection button in the list
                        grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;

                        //Enable or disable file and folder availability
                        grid_Popup_FilePicker_textblock_NoFilesAvailable.Visibility = Visibility.Collapsed;

                        //Enable or disable the side navigate buttons
                        grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Visible;
                        grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Collapsed;
                        grid_Popup_FilePicker_button_ControllerStart.Visibility = Visibility.Collapsed;

                        //Enable or disable the copy paste status
                        grid_Popup_FilePicker_textblock_ClipboardStatus.Visibility = Visibility.Collapsed;

                        //Enable or disable the current path
                        grid_Popup_FilePicker_textblock_CurrentPath.Visibility = Visibility.Collapsed;
                    });

                    //Add uwp applications to the filepicker list
                    await ListLoadAllUwpApplications(lb_FilePicker, List_FilePicker);
                }
                else
                {
                    //Clean the target path string
                    targetPath = Path.GetFullPath(targetPath);

                    //Add the Go up directory to the list
                    if (Path.GetPathRoot(targetPath) != targetPath)
                    {
                        BitmapImage imageBack = FileToBitmapImage(new string[] { "Assets/Default/Icons/Up.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        DataBindFile dataBindFileGoUp = new DataBindFile() { FileType = FileType.GoUpPre, Name = "Go up", Description = "Go up to the previous folder.", ImageBitmap = imageBack, PathFile = Path.GetDirectoryName(targetPath) };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileGoUp, false, false);
                    }
                    else
                    {
                        BitmapImage imageBack = FileToBitmapImage(new string[] { "Assets/Default/Icons/Up.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        DataBindFile dataBindFileGoUp = new DataBindFile() { FileType = FileType.GoUpPre, Name = "Go up", Description = "Go up to the previous folder.", ImageBitmap = imageBack, PathFile = "PC" };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileGoUp, false, false);
                    }

                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        //Enable or disable the copy paste status
                        if (vClipboardFiles.Any())
                        {
                            grid_Popup_FilePicker_textblock_ClipboardStatus.Visibility = Visibility.Visible;
                        }

                        //Enable or disable the current path
                        grid_Popup_FilePicker_textblock_CurrentPath.Text = "Current path: " + targetPath;
                        grid_Popup_FilePicker_textblock_CurrentPath.Visibility = Visibility.Visible;
                    });

                    //Add launch emulator options
                    if (vFilePickerShowRoms)
                    {
                        string fileDescription = "Launch the emulator without a rom loaded";
                        BitmapImage fileImage = FileToBitmapImage(new string[] { "Assets/Default/Icons/Emulator.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        DataBindFile dataBindFileWithoutRom = new DataBindFile() { FileType = FileType.FilePre, Name = fileDescription, Description = fileDescription + ".", ImageBitmap = fileImage, PathFile = string.Empty };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileWithoutRom, false, false);

                        string romDescription = "Launch the emulator with this folder as rom";
                        BitmapImage romImage = FileToBitmapImage(new string[] { "Assets/Default/Icons/Emulator.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                        DataBindFile dataBindFileFolderRom = new DataBindFile() { FileType = FileType.FilePre, Name = romDescription, Description = romDescription + ".", ImageBitmap = romImage, PathFile = targetPath };
                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFolderRom, false, false);
                    }

                    //Enable or disable the side navigate buttons
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_FilePicker_button_ControllerLeft.Visibility = Visibility.Visible;
                        grid_Popup_FilePicker_button_ControllerUp.Visibility = Visibility.Visible;
                        grid_Popup_FilePicker_button_ControllerStart.Visibility = Visibility.Visible;
                    });

                    //Get all the top files and folders
                    DirectoryInfo directoryInfo = new DirectoryInfo(targetPath);
                    DirectoryInfo[] directoryFolders = null;
                    FileInfo[] directoryFiles = null;
                    if (vFilePickerSortType == SortingType.Name)
                    {
                        directoryFolders = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).ToArray();
                        directoryFiles = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).OrderBy(x => x.Name).ToArray();
                    }
                    else
                    {
                        directoryFolders = directoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly).OrderByDescending(x => x.LastWriteTime).ToArray();
                        directoryFiles = directoryInfo.GetFiles("*", SearchOption.TopDirectoryOnly).OrderByDescending(x => x.LastWriteTime).ToArray();
                    }

                    //Get all rom images and descriptions
                    FileInfo[] directoryRomImages = new FileInfo[] { };
                    FileInfo[] directoryRomDescriptions = new FileInfo[] { };
                    if (vFilePickerShowRoms)
                    {
                        string[] imageFilter = { "jpg", "png" };
                        string[] descriptionFilter = { "json" };

                        DirectoryInfo directoryInfoRomsUser = new DirectoryInfo("Assets/User/Games");
                        FileInfo[] directoryPathsRomsUser = directoryInfoRomsUser.GetFiles("*", SearchOption.AllDirectories);
                        DirectoryInfo directoryInfoRomsDefault = new DirectoryInfo("Assets/Default/Games");
                        FileInfo[] directoryPathsRomsDefault = directoryInfoRomsDefault.GetFiles("*", SearchOption.AllDirectories);
                        IEnumerable<FileInfo> directoryPathsRoms = directoryPathsRomsUser.Concat(directoryPathsRomsDefault);

                        FileInfo[] romsImages = directoryPathsRoms.Where(file => imageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                        FileInfo[] filesImages = directoryFiles.Where(file => imageFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                        directoryRomImages = filesImages.Concat(romsImages).OrderByDescending(x => x.Name.Length).ToArray();

                        FileInfo[] romsDescriptions = directoryPathsRoms.Where(file => descriptionFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                        FileInfo[] filesDescriptions = directoryFiles.Where(file => descriptionFilter.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                        directoryRomDescriptions = filesDescriptions.Concat(romsDescriptions).OrderByDescending(x => x.Name.Length).ToArray();
                    }

                    //Get all the directories from target directory
                    if (vFilePickerShowDirectories)
                    {
                        try
                        {
                            //Fill the file picker listbox with folders
                            foreach (DirectoryInfo listFolder in directoryFolders)
                            {
                                try
                                {
                                    BitmapImage listImage = null;
                                    string listDescription = string.Empty;

                                    //Load image files for the list
                                    if (vFilePickerShowRoms)
                                    {
                                        GetRomDetails(listFolder.Name, listFolder.FullName, directoryRomImages, directoryRomDescriptions, ref listImage, ref listDescription);
                                    }
                                    else
                                    {
                                        listImage = FileToBitmapImage(new string[] { "Assets/Default/Icons/Folder.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, -1, 0);
                                    }

                                    //Get the folder size
                                    //string folderSize = AVFunctions.ConvertBytesSizeToString(GetDirectorySize(listDirectory));

                                    //Get the folder date
                                    string folderDate = listFolder.LastWriteTime.ToShortDateString().Replace("-", "/");

                                    //Set the detailed text
                                    string folderDetailed = folderDate;

                                    //Check the copy cut type
                                    ClipboardType clipboardType = ClipboardType.None;
                                    DataBindFile clipboardFile = vClipboardFiles.Where(x => x.PathFile == listFolder.FullName).FirstOrDefault();
                                    if (clipboardFile != null)
                                    {
                                        clipboardType = clipboardFile.ClipboardType;
                                    }

                                    //Add folder to the list
                                    bool systemFileFolder = listFolder.Attributes.HasFlag(FileAttributes.System);
                                    bool hiddenFileFolder = listFolder.Attributes.HasFlag(FileAttributes.Hidden);
                                    if (!systemFileFolder && (!hiddenFileFolder || Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowHiddenFilesFolders"))))
                                    {
                                        DataBindFile dataBindFileFolder = new DataBindFile() { FileType = FileType.Folder, ClipboardType = clipboardType, Name = listFolder.Name, NameDetail = folderDetailed, Description = listDescription, DateModified = listFolder.LastWriteTime, ImageBitmap = listImage, PathFile = listFolder.FullName };
                                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFolder, false, false);
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }

                    //Get all the files from target directory
                    if (vFilePickerShowFiles)
                    {
                        try
                        {
                            //Enable or disable selection button in the list
                            AVActions.ActionDispatcherInvoke(delegate
                            {
                                grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Collapsed;
                            });

                            //Filter files in and out
                            if (vFilePickerFilterIn.Any())
                            {
                                directoryFiles = directoryFiles.Where(file => vFilePickerFilterIn.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                            }
                            if (vFilePickerFilterOut.Any())
                            {
                                directoryFiles = directoryFiles.Where(file => !vFilePickerFilterOut.Any(filter => file.Name.EndsWith(filter, StringComparison.InvariantCultureIgnoreCase))).ToArray();
                            }

                            //Fill the file picker listbox with files
                            foreach (FileInfo listFile in directoryFiles)
                            {
                                try
                                {
                                    BitmapImage listImage = null;
                                    string listDescription = string.Empty;

                                    //Load image files for the list
                                    if (vFilePickerShowRoms)
                                    {
                                        GetRomDetails(listFile.Name, string.Empty, directoryRomImages, directoryRomDescriptions, ref listImage, ref listDescription);
                                    }
                                    else
                                    {
                                        string listFileFullNameLower = listFile.FullName.ToLower();
                                        string listFileExtensionLower = listFile.Extension.ToLower().Replace(".", string.Empty);
                                        if (listFileFullNameLower.EndsWith(".jpg") || listFileFullNameLower.EndsWith(".png") || listFileFullNameLower.EndsWith(".gif"))
                                        {
                                            listImage = FileToBitmapImage(new string[] { listFile.FullName }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 50, 0);
                                        }
                                        else
                                        {
                                            listImage = FileToBitmapImage(new string[] { "Assets/Default/Extensions/" + listFileExtensionLower + ".png", "Assets/Default/Icons/File.png" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 50, 0);
                                        }
                                    }

                                    //Get the file size
                                    string fileSize = AVFunctions.ConvertBytesSizeToString(listFile.Length);

                                    //Get the file date
                                    string fileDate = listFile.LastWriteTime.ToShortDateString().Replace("-", "/");

                                    //Set the detailed text
                                    string fileDetailed = fileSize + " (" + fileDate + ")";

                                    //Check the copy cut type
                                    ClipboardType clipboardType = ClipboardType.None;
                                    DataBindFile clipboardFile = vClipboardFiles.Where(x => x.PathFile == listFile.FullName).FirstOrDefault();
                                    if (clipboardFile != null)
                                    {
                                        clipboardType = clipboardFile.ClipboardType;
                                    }

                                    //Add file to the list
                                    bool systemFileFolder = listFile.Attributes.HasFlag(FileAttributes.System);
                                    bool hiddenFileFolder = listFile.Attributes.HasFlag(FileAttributes.Hidden);
                                    if (!systemFileFolder && (!hiddenFileFolder || Convert.ToBoolean(Setting_Load(vConfigurationCtrlUI, "ShowHiddenFilesFolders"))))
                                    {
                                        FileType fileType = FileType.File;
                                        string fileExtension = Path.GetExtension(listFile.Name);
                                        if (fileExtension == ".url" || fileExtension == ".lnk")
                                        {
                                            fileType = FileType.Link;
                                        }
                                        DataBindFile dataBindFileFile = new DataBindFile() { FileType = fileType, ClipboardType = clipboardType, Name = listFile.Name, NameDetail = fileDetailed, Description = listDescription, DateModified = listFile.LastWriteTime, ImageBitmap = listImage, PathFile = listFile.FullName };
                                        await ListBoxAddItem(lb_FilePicker, List_FilePicker, dataBindFileFile, false, false);
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        //Enable or disable selection button in the list
                        AVActions.ActionDispatcherInvoke(delegate
                        {
                            grid_Popup_FilePicker_button_SelectFolder.Visibility = Visibility.Visible;
                        });
                    }

                    //Check if there are files or folders
                    FilePicker_CheckFilesAndFoldersCount();
                }

                //Enable the file picker list
                AVActions.ActionDispatcherInvoke(delegate
                {
                    lb_FilePicker.IsEnabled = true;
                    gif_FilePicker_Loading.Hide();
                });

                //Focus on the file picker listbox
                await ListboxFocusIndex(lb_FilePicker, false, false, targetIndex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed loading filepicker: " + ex.Message);
                await FilePicker_Failed();
            }
        }

        //File Picker failed
        async Task FilePicker_Failed()
        {
            try
            {
                await Notification_Send_Status("Close", "Picker loading failed");
                await FilePicker_GoFolderUp();
            }
            catch { }
        }

        //File Picker check item
        void FilePicker_CheckItem()
        {
            try
            {
                DataBindFile dataBindFile = (DataBindFile)lb_FilePicker.SelectedItem;

                //Check the file or folder
                if (vFilePickerCurrentPath == "PC" || dataBindFile.FileType == FileType.FolderPre || dataBindFile.FileType == FileType.FilePre || dataBindFile.FileType == FileType.GoUpPre)
                {
                    Debug.WriteLine("Invalid file type, cannot be selected.");
                    return;
                }

                //Check or uncheck item
                if (dataBindFile.Checked != Visibility.Visible)
                {
                    dataBindFile.Checked = Visibility.Visible;
                }
                else
                {
                    dataBindFile.Checked = Visibility.Collapsed;
                }
            }
            catch { }
        }

        //Get rom details image and description
        void GetRomDetails(string listName, string listPath, FileInfo[] directoryRomImages, FileInfo[] directoryRomDescription, ref BitmapImage listImage, ref string listDescription)
        {
            try
            {
                //Set rom file names
                string romPathImage = string.Empty;
                string romPathDescription = string.Empty;
                string romNameFiltered = string.Empty;

                //Create sub directory image paths
                string subPathImagePng = string.Empty;
                string subPathImageJpg = string.Empty;
                string subPathDescription = string.Empty;
                if (!string.IsNullOrWhiteSpace(listPath))
                {
                    romNameFiltered = FilterNameRom(listName, false, true, false, 0);
                    subPathImagePng = Path.Combine(listPath, listName + ".png");
                    subPathImageJpg = Path.Combine(listPath, listName + ".jpg");
                    subPathDescription = Path.Combine(listPath, listName + ".txt");
                }
                else
                {
                    romNameFiltered = FilterNameRom(listName, true, true, false, 0);
                }

                //Check if rom directory has image
                foreach (FileInfo foundImage in directoryRomImages)
                {
                    try
                    {
                        string imageNameFiltered = FilterNameRom(foundImage.Name, true, true, false, 0);
                        //Debug.WriteLine(imageNameFiltered + " / " + romNameFiltered);
                        if (romNameFiltered.Contains(imageNameFiltered))
                        {
                            romPathImage = foundImage.FullName;
                            break;
                        }
                    }
                    catch { }
                }

                //Check if rom directory has description
                foreach (FileInfo foundDesc in directoryRomDescription)
                {
                    try
                    {
                        string descNameFiltered = FilterNameRom(foundDesc.Name, true, true, false, 0);
                        //Debug.WriteLine(descNameFiltered + " / " + romNameFiltered);
                        if (romNameFiltered.Contains(descNameFiltered))
                        {
                            romPathDescription = foundDesc.FullName;
                            break;
                        }
                    }
                    catch { }
                }

                //Update description and image
                listImage = FileToBitmapImage(new string[] { romPathImage, subPathImagePng, subPathImageJpg, "Rom" }, vImageSourceFolders, vImageBackupSource, IntPtr.Zero, 210, 0);
                string jsonFile = FileToString(new string[] { romPathDescription, subPathDescription });
                if (jsonFile.Contains("platform_logo"))
                {
                    ApiIGDBPlatformVersions platformVersionsJson = JsonConvert.DeserializeObject<ApiIGDBPlatformVersions>(jsonFile);
                    listDescription = ApiIGDB_ConsoleSummaryString(platformVersionsJson);
                }
                else
                {
                    ApiIGDBGames gamesJson = JsonConvert.DeserializeObject<ApiIGDBGames>(jsonFile);
                    listDescription = ApiIGDB_GameSummaryString(gamesJson);
                }
            }
            catch { }
        }

        //Check if there are files or folders
        void FilePicker_CheckFilesAndFoldersCount()
        {
            try
            {
                int totalFileCount = List_FilePicker.Count - 1; //Filter out GoUp
                if (totalFileCount > 0)
                {
                    //Enable or disable file and folder availability
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_FilePicker_textblock_NoFilesAvailable.Visibility = Visibility.Collapsed;
                    });
                    Debug.WriteLine("There are files and folders in the list.");
                }
                else
                {
                    //Enable or disable file and folder availability
                    AVActions.ActionDispatcherInvoke(delegate
                    {
                        grid_Popup_FilePicker_textblock_NoFilesAvailable.Visibility = Visibility.Visible;
                    });
                    Debug.WriteLine("None files and folders in the list.");
                }
            }
            catch { }
        }

        //Close file picker popup
        async Task Popup_Close_FilePicker(bool IsCompleted, bool CurrentFolder)
        {
            try
            {
                PlayInterfaceSound(vConfigurationCtrlUI, "PopupClose", false);

                //Reset and update popup variables
                vFilePickerOpen = false;
                if (IsCompleted)
                {
                    vFilePickerCompleted = true;
                    if (CurrentFolder)
                    {
                        DataBindFile targetPath = new DataBindFile();
                        targetPath.PathFile = vFilePickerCurrentPath;
                        vFilePickerResult = targetPath;
                    }
                    else
                    {
                        vFilePickerResult = (DataBindFile)lb_FilePicker.SelectedItem;
                    }
                }
                else
                {
                    vFilePickerCancelled = true;
                }

                //Store the current picker path
                if (vFilePickerCurrentPath.Contains(":"))
                {
                    Debug.WriteLine("Closed the picker on: " + vFilePickerCurrentPath);
                    vFilePickerPreviousPath = vFilePickerCurrentPath;
                }

                //Clear the current file picker list
                vFilePickerNavigateIndexes.Clear();
                List_FilePicker.Clear();

                //Hide the popup
                Popup_Hide_Element(grid_Popup_FilePicker);

                //Focus on the previous focus element
                await Popup_PreviousElementFocus_Focus(vFilePickerElementFocus);
            }
            catch { }
        }
    }
}