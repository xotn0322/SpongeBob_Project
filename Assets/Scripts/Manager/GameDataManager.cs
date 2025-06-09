using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GameDataManager : IEngineComponent
{
    public static GameDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameDataManager();
            }
            return _instance;
        }
    }

    private static GameDataManager _instance;

    private Dictionary<Type, IDataManager> _gameDataManagers;
    public IEngineComponent Init()
    {
        _gameDataManagers = new Dictionary<Type, IDataManager>
        {
            {typeof(IngredientData), IngredientDataManager.Instance}
        };

        return this;
    }

    public async Task LoadDataManager()
    {
        foreach (var dataManager in _gameDataManagers.Values)
        {
            await Task.Run(() => dataManager.LoadDataFromJson());
        }
    }
}