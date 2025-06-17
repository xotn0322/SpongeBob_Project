using System.Data.Common;
using UnityEngine;

public class PlayerDataManager : IDataManager
{
    public static PlayerDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerDataManager();
            }
            return _instance;
        }
    }

    private static PlayerDataManager _instance;


    //private
    PlayerData playerData;
    private string _defaultPath = IOUtil.CombinePath(Constant.Path.DEFAULT_DATA_PATH, "PlayerData/");
    private string _json;

    //public

    //function
    public string GetDefaultFilePath()
    {
        return _defaultPath;
    }
    public void LoadDataFromJson()
    {
        SetJsonFilePath();

        var list = FileIOManager.Instance.LoadJsonList<PlayerData>(_json);
        if (list != null && list.Count > 0)
        {
            foreach(var data in list)
            {
                playerData = data;
            }
        }
        else
        {
            Debug.LogWarning("Player data not found or failed to load. Creating default player data.");
            playerData = new PlayerData { id = "Player1", Hp = 100 }; // 기본값 설정
        }
    }

    private void SetJsonFilePath()
    {
        _json = IOUtil.CombinePath(_defaultPath, "PlayerData.json");
    }

    public PlayerData GetData()
    {
        return playerData;
    }
}