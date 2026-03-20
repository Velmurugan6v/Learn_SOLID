using UnityEngine;
using System.IO;

public static class SaveManager
{
    static string SaveDataPath => Application.persistentDataPath + "/saveData.json";


    public static void SaveData(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SaveDataPath, json);

        Debug.Log($"Saved to : {SaveDataPath}");
    }

    public static SaveData LoadData()
    {
        if (!File.Exists(SaveDataPath))
            return new SaveData();

        string json = File.ReadAllText(SaveDataPath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void DeleteSaveData()
    {
        if (File.Exists(SaveDataPath))
            File.Delete(SaveDataPath);
    }
}

[System.Serializable]
public class SaveData
{
    public string playerName;
    public int currentLevel;
}