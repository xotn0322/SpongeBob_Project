using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class FileIOManager : IEngineComponent
{
    private static FileIOManager _instance;
    public static FileIOManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new FileIOManager();
            }
            return _instance;
        }
    }
    public IEngineComponent Init()
    {
        return this;
    }
    public List<T> LoadJsonList<T>(string path)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<T>>(json);
        }
        return null;
    }
    
    public List<T> LoadJsonListByEnum<T>(string path, System.Enum enumValue)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, List<T>>>(json);
            
            if (jsonObject != null && jsonObject.ContainsKey(enumValue.ToString()))
            {
                return jsonObject[enumValue.ToString()];
            }
        }
        return null;
    }
} 