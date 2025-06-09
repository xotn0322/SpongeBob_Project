using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IngredientManager : MonoBehaviour, IEngineComponent
{
    public static IngredientManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private static IngredientManager _instance;

    public IEngineComponent Init()
    {
        SetPosition();
        InitSpawn();

        return this;
    }

    //private
    private Dictionary<int, GameObject> ingredients = new Dictionary<int, GameObject>();

    private List<Vector3> ingredientPosition = new List<Vector3>();

    //public

    
    //functions

    public GameObject SpawnPrefab(EIngredientName prefab, Vector3 globalPositon)
    {
        var resource = ResourcesManager.Instance.Load<GameObject>(IOUtil.CombinePath(Constant.Path.RESOURCE_INGREDIENT_PATH, prefab.ToString()));
        var gameObject = Instantiate(resource);
        var ingredientComponent = gameObject.GetComponent<IngredientComponent>();
        gameObject.transform.position = globalPositon;
        ingredientComponent.Init();
        ingredients.Add(ingredients.Count, gameObject);
 
        return gameObject;
    }

    private void InitSpawn()
    {
        SpawnPrefab(EIngredientName.Bottom_Bun, ingredientPosition[0]);
        SpawnPrefab(EIngredientName.Tomato, ingredientPosition[1]);
        SpawnPrefab(EIngredientName.Tomato, ingredientPosition[2]);
        SpawnPrefab(EIngredientName.Top_Bun, ingredientPosition[3]);
    }

    private void SetPosition()
    {
        ingredientPosition.Add(new Vector3(-93.8539963f,1.21399999f,0.75f));
        ingredientPosition.Add(new Vector3(-93.8050003f,1.45799994f,1.15799999f));
        ingredientPosition.Add(new Vector3(-93.8050003f,2f,1.15799999f));
        ingredientPosition.Add(new Vector3(-93.8050003f,1.45799994f,1.45799999f));
    }
}