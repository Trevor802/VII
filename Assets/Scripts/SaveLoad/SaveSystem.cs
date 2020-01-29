using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem{
    public static void SavePlayer(Player player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        SavePlayerData data = new SavePlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SavePlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.save";
        //Debug.Log(path);
        if(File.Exists(path))
        {
            //Debug.Log("Load success");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SavePlayerData data = formatter.Deserialize(stream) as SavePlayerData;
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }
    
    public static void DeleteSave()
    {
        string path = Application.persistentDataPath + "/player.save";
        File.Delete(path);
    }
}
