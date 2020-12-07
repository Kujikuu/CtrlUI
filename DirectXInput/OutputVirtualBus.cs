﻿using System.Diagnostics;
using System.Runtime.InteropServices;
using static DirectXInput.AppVariables;
using static LibraryShared.Classes;
using static LibraryUsb.WinUsbDevice;

namespace DirectXInput
{
    public partial class WindowMain
    {
        //Receive and send rumble
        void LoopOutput(ControllerStatus Controller)
        {
            try
            {
                Debug.WriteLine("Receive and send rumble for: " + Controller.Details.DisplayName);

                //Initialize controller
                ControllerInitialize(Controller);

                //Send default output to controller
                ControllerOutput(Controller, true, false, false);

                //Receive output from the virtual bus
                while (!Controller.OutputTask.TaskStopRequest && Controller.Connected())
                {
                    try
                    {
                        //Set receive structure
                        Controller.XOutputData = new XUSB_OUTPUT_REPORT();
                        Controller.XOutputData.Size = Marshal.SizeOf(Controller.XOutputData);
                        Controller.XOutputData.SerialNo = Controller.NumberId + 1;

                        //Receive output from the virtual bus
                        vVirtualBusDevice.VirtualOutput(ref Controller);

                        //Send output to the controller
                        ControllerOutput(Controller, false, false, false);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}