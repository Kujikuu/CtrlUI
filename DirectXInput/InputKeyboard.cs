﻿using System.Collections.Generic;
using System.Diagnostics;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVSettings;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    public partial class WindowMain
    {
        private async void EventHotKeyPressed(List<KeysVirtual> keysPressed)
        {
            try
            {
                bool altPressed = keysPressed.Contains(KeysVirtual.AltLeft);
                bool controlPressed = keysPressed.Contains(KeysVirtual.ControlLeft);
                bool shiftPressed = keysPressed.Contains(KeysVirtual.ShiftLeft);
                bool windowsPressed = keysPressed.Contains(KeysVirtual.WindowsLeft);
                bool modifierKeyPressed = altPressed || controlPressed || shiftPressed || windowsPressed;

                if (windowsPressed && keysPressed.Contains(KeysVirtual.OEMTilde))
                {
                    //Launch or show CtrlUI
                    if (SettingLoad(vConfigurationDirectXInput, "ShortcutLaunchCtrlUIKeyboard", typeof(bool)))
                    {
                        Debug.WriteLine("Button Global - Show or hide CtrlUI");
                        await ToolFunctions.CtrlUI_LaunchShow();
                    }
                }
            }
            catch { }
        }
    }
}