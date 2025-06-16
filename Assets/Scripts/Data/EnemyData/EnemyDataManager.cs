using System.Collections.Generic;

public class EnemyDataManager : IDataManager, IDataManager<EEnemyName, EnemyData>
{
    public static EnemyDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EnemyDataManager();
            }
            return _instance;
        }
    }

    private static EnemyDataManager _instance;

    private string _defaultPath = IOUtil.CombinePath(Constant.Path.DEFAULT_DATA_PATH, "EnemyDatas/");
    private string _json;
    private Dictionary<EEnemyName, EnemyData> _EnemyData = new Dictionary<EEnemyName, EnemyData>();

    public Dictionary<EEnemyName, EnemyData> GetDataDictionry()
    {
        return _EnemyData;
    }

    public EnemyData GetData(EEnemyName key)
    {
        return _EnemyData[key];
    }

    public string GetDefaultFilePath()
    {
        return _defaultPath;
    }

    public void LoadDataFromJson()
    {
        SetJsonFilePath();

        var list = FileIOManager.Instance.LoadJsonList<EnemyData>(_json);
        foreach(var data in list)
        {
            _EnemyData.Add(data.EnemyName, data);
        }
    }

    private void SetJsonFilePath()
    {
        _json = IOUtil.CombinePath(_defaultPath, "EnemyDatas.json");
    }
}
