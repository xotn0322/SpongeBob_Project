using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Resources;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private static GameManager _instance;

    //private
    private bool _LoadComplete = false;
    private float _LoadProgress = 0f;
    private string _LoadProgressText;

    //public


    //functions

    private Dictionary<int, IEngineComponent>_engineComponents = new Dictionary<int, IEngineComponent>()
    {
        {150, ResourcesManager.Instance },
        {1100, FileIOManager.Instance},
        {100000, GameDataManager.Instance}
    };

    private Dictionary<int, Type>_monoBehaviorEngineComponents = new Dictionary<int, Type>()
    {
        {550000, typeof(TimeManager)},
        {560000, typeof(IngredientManager)},
        {570000, typeof(EnemyManager)}
    };

    private void Awake()
    {
        // 이미 인스턴스가 존재하는 경우
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);  // this 대신 gameObject 사용

        foreach (var engineComponent in _engineComponents)
        {
            engineComponent.Value.Init();
        }
    }

    public async void LoadGame() //SceneManager에서 관리
    {
        _LoadComplete = false;

        Debug.Log("Start Loading Game...");
        _LoadProgress = 0f;
        _LoadProgressText = "초기화 중...";

        Debug.Log("Load Game Data...");
        _LoadProgress = 0.55f;
        _LoadProgressText = "게임 데이터 로딩 중...";
        await GameDataManager.Instance.LoadDataManager();
        await Task.Delay(200);

        _LoadProgress = 0.99f;
        _LoadProgressText = "리소스 적용 중...";
        await Task.Delay(300);
        
        _LoadComplete = true;
    }

    public bool IsLoadComplete()
    {
        return _LoadComplete;
    }

    public float GetLoadProgress()
    {
        return _LoadProgress;
    }

    public string GetLoadProgressText()
    {
        return _LoadProgressText;
    }

    public void InitMonoBehaviourGameEngine()
    {
        LoadMonoBehaviourEngineComponent();
    }

    private void LoadMonoBehaviourEngineComponent()
    {
        foreach(var engineComponent in _monoBehaviorEngineComponents)
        {
            var resource = ResourcesManager.Instance.Load<GameObject>(IOUtil.CombinePath(Constant.Path.RESOURCE_ENGINECOMPONENT_PATH, engineComponent.Value.Name));
            var gameObject = Instantiate(resource);
            var monoBehaviourEngineComponent = (IEngineComponent)gameObject.GetComponent(engineComponent.Value);
            monoBehaviourEngineComponent.Init();

            _engineComponents.Add(engineComponent.Key, monoBehaviourEngineComponent);
        }
        _monoBehaviorEngineComponents.Clear();
    }

    public void StartGame()
    {
        Debug.Log("Game Loading Complete!");

        StartGameInternal();
    }

    public void StartGameInternal()
    {
        
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Destroy(gameObject);
        Application.Quit();
#endif
    }
}