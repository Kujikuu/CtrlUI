﻿using System;
using static ArnoldVinkCode.ArnoldVinkSockets;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Check incoming gyro dsu client
        bool GyroDsuClientHandler(UdpEndPointDetails endPoint, byte[] incomingBytes)
        {
            try
            {
                //Check gyro dsuc header
                if (incomingBytes[0] != 'D' && incomingBytes[1] != 'S' && incomingBytes[2] != 'U' && incomingBytes[3] != 'C')
                {
                    return false;
                }

                //Debug.WriteLine("Gyro dsu client connected: " + endPoint.IPEndPoint.Address + ":" + endPoint.IPEndPoint.Port);

                //Check gyro message type
                DsuMessageType messageType = (DsuMessageType)BitConverter.ToUInt32(incomingBytes, 16);
                if (messageType == DsuMessageType.DSUC_PadDataReq)
                {
                    //Check gyro controller id
                    byte controllerId = incomingBytes[21];

                    //Update gyro dsu client endpoints
                    if (controllerId == 0) { vController0.GyroDsuClientEndPoint = endPoint; }
                    if (controllerId == 1) { vController1.GyroDsuClientEndPoint = endPoint; }
                    if (controllerId == 2) { vController2.GyroDsuClientEndPoint = endPoint; }
                    if (controllerId == 3) { vController3.GyroDsuClientEndPoint = endPoint; }
                }

                return true;
            }
            catch { }
            return false;
        }
    }
}