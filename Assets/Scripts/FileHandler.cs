using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class FileHandler
{
    private string DirPath = "";
    private string GameDataFileName = "";

    public FileHandler(string DirPath, string GameDataFileName = null)
    {
        this.DirPath = DirPath;
        this.GameDataFileName = GameDataFileName;
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(DirPath, GameDataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }


    public void LoadCountryInfo(string filename, List<Country> countryList)
    {
        TextAsset textFile = Resources.Load<TextAsset>(filename);
        if (textFile != null)
        {    
            List<string> addedList = new List<string>();
                        
            var text = textFile.text;
            var lines = text.Split('\n');

            foreach (var line in lines)
            {
                var values = line.Split(',');
                foreach (var country in countryList)
                {
                    if (country.name == values[0])
                    {
                        country.infoList.Add(new Info(int.Parse(values[1]), float.Parse(values[3])));
                        addedList.Add(country.name);
                    }
                }
            }  

            foreach(var country in countryList)
            {
                if (!addedList.Contains(country.name))
                {
                    country.infoList.Add(new Info(0, 0.0f));
                }
            } 
        }
    }

    public void Save(GameData data)
    {
        string fullPath = Path.Combine(DirPath, GameDataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
        }
    }

    
}
