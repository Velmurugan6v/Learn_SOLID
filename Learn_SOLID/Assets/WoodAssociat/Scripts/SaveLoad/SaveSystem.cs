using System;
using System.IO;
using UnityEngine;
using WoodAssociat.Utility;

namespace WoodAssociat
{
    public class SaveSystem : MonoBehaviour
    {
        private string _savePath;
        [SerializeField] private int levelIndicatorValue;

        private void Awake()
        {
            _savePath = Application.persistentDataPath + "/Save.json";
            Loghandler.Log(_savePath);
        }

         
        [ContextMenu("Save")]
        public void SaveGameData(ISaveble saveData)
        {
            
        }

        [ContextMenu("Load")]
        public void LoadGameData()
        {
            if (File.Exists(_savePath))
            {
                var json = File.ReadAllText(_savePath);

                SaveData data = JsonUtility.FromJson<SaveData>(json);

                levelIndicatorValue = data.recentlyPlayingLevelId;
                Loghandler.Log(data.name);
                Loghandler.Log(data.recentlyPlayingLevelId);
            }
        }
    }


    [Serializable]
    public class SaveData
    {
        public string name;
        public int recentlyPlayingLevelId;
    }
}