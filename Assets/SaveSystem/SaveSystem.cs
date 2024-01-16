using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class SaveSystem
{
    private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";
    public const string FILE_NAME = "SaveFile";
    private const string SAVE_EXTENTION = ".sav";
    public static string fileName { get; private set; }
    public static string filePath { get; private set; }

    public static void Initialize()
    {
        //check if the folder doesn't exist
        if (!Directory.Exists(SAVE_FOLDER))
        {
            //create the directory
            Directory.CreateDirectory(SAVE_FOLDER);
        }

        //initialize the file name and path
        fileName = FILE_NAME + SAVE_EXTENTION;
        filePath = SAVE_FOLDER + FILE_NAME + SAVE_EXTENTION;
    }

    public static void Save(SaveData saveObject)
    {
        //to prevent errors
        var settings = new JsonSerializerSettings();
        //this tells your serializer that multiple references are okay
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        //save object converted to a JSON string
        string saveString = JsonConvert.SerializeObject(saveObject, settings);
        Debug.Log("Saved string: " + saveString);
        //write the string into a file
        File.WriteAllText(filePath, saveString);
    }

    public static SaveData Load()
    {
        //check if the file exists
        if (File.Exists(filePath))
        {
            //get the JSON string
            string saveString = File.ReadAllText(filePath);
            
            Debug.Log("Loaded string: " + saveString);
            //get the object from the saved string
            SaveData loaded = JsonConvert.DeserializeObject<SaveData>(saveString);
            //check if the object was not loaded
            if (loaded == null)
            {
                //return the new data
                return new SaveData();
            }

            return loaded;
        }
        else
        {
            //return the new data
            return new SaveData();
        }
    }
}
