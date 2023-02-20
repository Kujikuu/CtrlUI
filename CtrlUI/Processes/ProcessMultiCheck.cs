﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVImage;
using static ArnoldVinkCode.AVProcess;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Check process status before launching (True = Continue)
        async Task CheckLaunchProcessStatus(DataBindApp dataBindApp, ProcessMulti processMulti)
        {
            try
            {
                Debug.WriteLine("Checking launch process: " + dataBindApp.Name);

                //Focus or Close when process is already running
                List<DataBindString> Answers = new List<DataBindString>();
                DataBindString AnswerShow = new DataBindString();
                AnswerShow.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppMiniMaxi.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerShow.Name = "Show application";
                Answers.Add(AnswerShow);

                DataBindString AnswerClose = new DataBindString();
                AnswerClose.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppClose.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerClose.Name = "Close application";
                Answers.Add(AnswerClose);

                DataBindString AnswerLaunch = new DataBindString();
                AnswerLaunch.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppLaunch.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerLaunch.Name = "Launch new instance";
                Answers.Add(AnswerLaunch);

                DataBindString AnswerRestartCurrent = new DataBindString();
                if (!string.IsNullOrWhiteSpace(processMulti.Argument))
                {
                    AnswerRestartCurrent.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                    AnswerRestartCurrent.Name = "Restart application";
                    AnswerRestartCurrent.NameSub = "(Current argument)";
                    Answers.Add(AnswerRestartCurrent);
                }

                DataBindString AnswerRestartWithout = new DataBindString();
                AnswerRestartWithout.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/AppRestart.png" }, null, vImageBackupSource, IntPtr.Zero, -1, 0);
                AnswerRestartWithout.Name = "Restart application";
                if (!string.IsNullOrWhiteSpace(dataBindApp.Argument) || dataBindApp.Category == AppCategory.Shortcut || dataBindApp.Category == AppCategory.Emulator || dataBindApp.LaunchFilePicker)
                {
                    AnswerRestartWithout.NameSub = "(Default argument)";
                }
                else
                {
                    AnswerRestartWithout.NameSub = "(Without argument)";
                }
                Answers.Add(AnswerRestartWithout);

                //Get process details
                string processDetails = dataBindApp.PathExe;
                if (!string.IsNullOrWhiteSpace(dataBindApp.NameExe))
                {
                    processDetails += " (" + dataBindApp.NameExe + ")";
                }

                //Get process running time and last launch time
                string processRunningTimeString = string.Empty;
                string lastLaunchTimeString = string.Empty;
                if (dataBindApp.Category == AppCategory.Shortcut)
                {
                    processRunningTimeString = ApplicationRunningTimeString(dataBindApp.RunningTime, "shortcut process");
                }
                else
                {
                    processRunningTimeString = ApplicationRunningTimeString(dataBindApp.RunningTime, "application");
                    lastLaunchTimeString = ApplicationLastLaunchTimeString(dataBindApp.LastLaunch, "Application");
                }

                //Set the running time string
                bool runningTimeEmpty = string.IsNullOrWhiteSpace(processRunningTimeString);
                bool launchTimeEmpty = string.IsNullOrWhiteSpace(lastLaunchTimeString);
                if (runningTimeEmpty && launchTimeEmpty)
                {
                    processRunningTimeString = processDetails;
                }
                else
                {
                    if (!launchTimeEmpty)
                    {
                        processRunningTimeString += "\n" + lastLaunchTimeString;
                    }
                    processRunningTimeString += "\n" + processDetails;
                }

                //Show the messagebox
                DataBindString messageResult = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", processRunningTimeString, "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerShow)
                    {
                        await ShowProcessWindow(dataBindApp, processMulti);
                    }
                    else if (messageResult == AnswerClose)
                    {
                        await CloseSingleProcessAuto(processMulti, dataBindApp, true, false);
                    }
                    else if (messageResult == AnswerRestartCurrent)
                    {
                        await RestartProcessAuto(processMulti, dataBindApp, true);
                    }
                    else if (messageResult == AnswerRestartWithout)
                    {
                        await RestartProcessAuto(processMulti, dataBindApp, false);
                    }
                    else if (messageResult == AnswerLaunch)
                    {
                        await LaunchProcessDatabindAuto(dataBindApp);
                    }
                }
                else
                {
                    Debug.WriteLine("Cancelling the process action.");
                }
            }
            catch (Exception ex)
            {
                await Notification_Send_Status("Close", "Failed showing or closing application");
                Debug.WriteLine("Failed closing or showing the application: " + ex.Message);
            }
        }
    }
}