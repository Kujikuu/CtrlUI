﻿using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Download igdb genres
        public async Task<bool> ApiIGDBDownloadGenres()
        {
            try
            {
                Debug.WriteLine("Downloading IGDB genres.");

                //Authenticate with Twitch
                string authAccessToken = await ApiTwitch_Authenticate();
                if (string.IsNullOrWhiteSpace(authAccessToken))
                {
                    return false;
                }

                //Set request headers
                string[] requestAccept = new[] { "Accept", "application/json" };
                string[] requestClientID = new[] { "Client-ID", vApiIGDBClientID };
                string[] requestAuthorization = new[] { "Authorization", "Bearer " + authAccessToken };
                string[][] requestHeaders = new string[][] { requestAccept, requestClientID, requestAuthorization };

                //Create request uri
                Uri requestUri = new Uri("https://api.igdb.com/v4/genres");

                //Create request body
                string requestBodyString = "fields *; sort id asc;";
                StringContent requestBodyStringContent = new StringContent(requestBodyString, Encoding.UTF8, "application/text");

                //Download igdb genres
                string resultSearch = await AVDownloader.SendPostRequestAsync(5000, "CtrlUI", requestHeaders, requestUri, requestBodyStringContent);
                if (string.IsNullOrWhiteSpace(resultSearch))
                {
                    Debug.WriteLine("Failed downloading IGDB genres.");
                    return false;
                }

                //Save igdb genres
                File.WriteAllText("Resources/ApiIGDB/Genres.json", resultSearch);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed downloading IGDB genres: " + ex.Message);
                return false;
            }
        }
    }
}