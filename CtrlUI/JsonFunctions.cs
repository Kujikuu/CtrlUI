﻿using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Read apps from Json file (Deserialize)
        async Task JsonLoadList_Applications()
        {
            try
            {
                //Remove all the current apps
                List_Games.Clear();
                List_Apps.Clear();
                List_Emulators.Clear();

                //Add all the apps to the list
                string JsonFile = File.ReadAllText(@"Profiles\CtrlApplications.json");
                DataBindApp[] JsonList = JsonConvert.DeserializeObject<DataBindApp[]>(JsonFile).OrderBy(x => x.Number).ToArray();
                foreach (DataBindApp dataBindApp in JsonList)
                {
                    try
                    {
                        await AddAppToList(dataBindApp, false, true);
                    }
                    catch { }
                }

                Debug.WriteLine("Reading Json applications completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed reading Json applications: " + ex.Message);
            }
        }

        //Read Json from profile (Deserialize)
        void JsonLoadProfile<T>(ref T deserializeTarget, string profileName)
        {
            try
            {
                string jsonFile = File.ReadAllText(@"Profiles\" + profileName + ".json");
                deserializeTarget = JsonConvert.DeserializeObject<T>(jsonFile);
                Debug.WriteLine("Reading Json file completed: " + profileName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Reading Json file failed: " + profileName + "/" + ex.Message);
            }
        }

        //Read Json from embedded file (Deserialize)
        void JsonLoadEmbedded<T>(ref T deserializeTarget, string resourcePath)
        {
            try
            {
                string jsonFile = string.Empty;
                Assembly assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        jsonFile = reader.ReadToEnd();
                    }
                }

                deserializeTarget = JsonConvert.DeserializeObject<T>(jsonFile);
                Debug.WriteLine("Reading Json resource completed: " + resourcePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Reading Json resource failed: " + resourcePath + "/" + ex.Message);
            }
        }

        //Save to Json file (Serialize)
        void JsonSaveObject(object serializeObject, string profileName)
        {
            try
            {
                //Json settings
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                jsonSettings.NullValueHandling = NullValueHandling.Ignore;

                //Json serialize
                string serializedObject = JsonConvert.SerializeObject(serializeObject, jsonSettings);

                //Save to file
                File.WriteAllText(@"Profiles\" + profileName + ".json", serializedObject);
                Debug.WriteLine("Saving Json " + profileName + " completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed saving Json " + profileName + ": " + ex.Message);
            }
        }

        //Save to Json file (Serialize)
        void JsonSaveApplications()
        {
            try
            {
                //Combine apps
                var JsonFilterList = CombineAppLists(false, false).Select(x => new { x.Number, x.Category, x.Type, x.Name, x.NameExe, x.PathImage, x.PathExe, x.PathLaunch, x.PathRoms, x.Argument, x.QuickLaunch, x.LaunchFilePicker, x.LaunchKeyboard, x.RunningTime });

                //Json settings
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                jsonSettings.NullValueHandling = NullValueHandling.Ignore;

                //Json serialize
                string serializedList = JsonConvert.SerializeObject(JsonFilterList, jsonSettings);

                //Save to file
                File.WriteAllText(@"Profiles\CtrlApplications.json", serializedList);
                Debug.WriteLine("Saving Json apps completed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed saving Json apps: " + ex.Message);
            }
        }
    }
}