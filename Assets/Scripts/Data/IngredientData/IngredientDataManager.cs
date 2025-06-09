using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;

public class IngredientDataManager : IDataManager, IDataManager<EIngredientName, IngredientData>
{
    public static IngredientDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new IngredientDataManager();
            }
            return _instance;
        }
    }

    private static IngredientDataManager _instance;
    private string _defaultPath = IOUtil.CombinePath(Constant.Path.DEFAULT_DATA_PATH, "IngredientDatas/");
    private string _json;
    private Dictionary<EIngredientName, IngredientData> _IngredientData = new Dictionary<EIngredientName, IngredientData>();

    public Dictionary<EIngredientName, IngredientData> GetDataDictionry()
    {
        return _IngredientData;
    }

    public IngredientData GetData(EIngredientName key)
    {
        return _IngredientData[key];
    }

    public string GetDefaultFilePath()
    {
        return _defaultPath;
    }

    public void LoadDataFromJson()
    {
        SetJsonFilePath();

        var list = FileIOManager.Instance.LoadJsonList<IngredientData>(_json);
        foreach(var data in list)
        {
            _IngredientData.Add(data.IngredientName, data);
        }
    }

    private void SetJsonFilePath()
    {
        _json = IOUtil.CombinePath(_defaultPath, "IngredientDatas.json");
    }
}
