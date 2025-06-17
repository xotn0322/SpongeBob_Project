using UnityEngine;
using System.Collections.Generic;
using System.Resources;

public class EnemyManager : MonoBehaviour, IEngineComponent
{
    public static EnemyManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private static EnemyManager _instance;

    public IEngineComponent Init()
    {
        return this;
    }

    //private
    private Dictionary<int, GameObject> enemys = new Dictionary<int, GameObject>();

    //public

    
    //functions

    public GameObject SpawnPrefab(EEnemyName prefab, Vector3 globalPositon)
    {
        var resource = ResourcesManager.Instance.Load<GameObject>(IOUtil.CombinePath(Constant.Path.RESOURCE_ENEMY_PATH, prefab.ToString()));
        var gameObject = Instantiate(resource);
        var enemyComponent = gameObject.GetComponent<EnemyComponent>();
        gameObject.transform.position = globalPositon;
        enemyComponent.Init();
        enemys.Add(enemys.Count, gameObject);
 
        return gameObject;
    }
}