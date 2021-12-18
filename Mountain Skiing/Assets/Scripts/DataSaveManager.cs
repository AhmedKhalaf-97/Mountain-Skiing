using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class DataSaveManager
{
    static string saveLocation = Application.persistentDataPath + "/PlayerSave/";
    static string saveFileName = "savefile.AYK";

    static Hashtable hashtable = new Hashtable();

    static void CreateSaveLocation()
    {
        if (!Directory.Exists(saveLocation))
        {
            Directory.CreateDirectory(saveLocation);
        }
    }

    public static void DeleteAllData()
    {
        if (Directory.Exists(saveLocation))
        {
            Directory.Delete(saveLocation, true);
        }
    }

    public static bool IsDataExist(string key)
    {
        if (File.Exists(saveLocation + saveFileName))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(saveLocation + saveFileName, FileMode.Open);

            object data = binaryFormatter.Deserialize(fileStream);

            hashtable = (Hashtable)data;

            fileStream.Close();

            if (hashtable.ContainsKey(key))
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }

    public static void DeleteData(string key)
    {
        if (File.Exists(saveLocation + saveFileName))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(saveLocation + saveFileName, FileMode.Open);

            if (hashtable.ContainsKey(key))
                hashtable.Remove(key);

            binaryFormatter.Serialize(fileStream, hashtable);
            fileStream.Close();
        }
    }

    public static void SaveData(string key, object data)
    {
        CreateSaveLocation();

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(saveLocation + saveFileName, FileMode.OpenOrCreate);

        if (hashtable.ContainsKey(key))
            hashtable[key] = data;
        else
            hashtable.Add(key, data);

        binaryFormatter.Serialize(fileStream, hashtable);
        fileStream.Close();
    }

    public static object LoadData(string key)
    {
        if (File.Exists(saveLocation + saveFileName))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(saveLocation + saveFileName, FileMode.Open);

            object data = binaryFormatter.Deserialize(fileStream);

            hashtable = (Hashtable)data;

            fileStream.Close();
            return hashtable[key];
        }
        else
        {
            return null;
        }
    }
}