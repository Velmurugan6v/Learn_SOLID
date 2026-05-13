using System;
using System.Collections.Generic;
using UnityEngine;

namespace WoodAssociat
{
    [Serializable]
    public class CSVData
    {
        public string dataName;
        public int dataId;
    }
    
    public class CSVDataLoader : MonoBehaviour
    {
        public List<CSVData> csvDatas = new List<CSVData>();


        private void Start()
        {
            LoadCSVData();
        }

        void LoadCSVData()
        {
            TextAsset csvFile = Resources.Load("DataLoad") as TextAsset;
            
            string[] lines = csvFile.text.Split('\n');

            for (int i = 1; i < lines.Length; i++)
            {
                if(string.IsNullOrWhiteSpace(lines[i]))
                    continue;
                
                string[] data = lines[i].Split(',');
                
                CSVData csvData = new CSVData();
                
                csvData.dataName = data[0];
                csvData.dataId = int.Parse(data[1]);
                
                csvDatas.Add(csvData);
            }

            PrintData();
        }
        
        void PrintData()
        {
            foreach (CSVData data in csvDatas)
            {
                Debug.Log($"Name: {data.dataName} | ID: {data.dataId}");
            }
        }
    }
}
