using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Resources;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;


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
        {550000, typeof(IngredientManager)},
        {560000, typeof(EnemyManager)}
    };

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);

        foreach (var engineComponent in _engineComponents)
        {
            engineComponent.Value.Init();
        }

        // LoadGame();
        // InitMonoBehaviourGameEngine();
        StartCoroutine(LoadSceneProcess());
    }

    public IEnumerator LoadGameProcess() //SceneManager에서 관리
    {
        _LoadComplete = false;

        Debug.Log("Start Loading Game...");
        _LoadProgress = 0f;
        _LoadProgressText = "초기화 중...";

        Debug.Log("Load Game Data...");
        _LoadProgress = 0.55f;
        _LoadProgressText = "게임 데이터 로딩 중...";
        var loadDataTask = GameDataManager.Instance.LoadDataManager();
        while (!loadDataTask.IsCompleted)
        {
            yield return null; // Wait for a frame
        }
        if (loadDataTask.IsFaulted)
        {
            Debug.LogError($"Error loading game data: {loadDataTask.Exception}");
            yield break;
        }
        yield return new WaitForSeconds(0.2f);

        _LoadProgress = 0.99f;
        _LoadProgressText = "리소스 적용 중...";
        yield return new WaitForSeconds(0.3f);

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

    IEnumerator LoadSceneProcess()
{
    // Wait 2 frames for LoadingScene
    yield return new WaitForEndOfFrame();
    yield return new WaitForEndOfFrame();

    yield return LoadGameProcess();

    // Wait 2 frames for applying new Scene
    yield return new WaitForEndOfFrame();
    yield return new WaitForEndOfFrame();

    GameManager.Instance.InitMonoBehaviourGameEngine();

    // Wait 1 frame for MonoBehaviour GameEngine Load
    yield return new WaitForEndOfFrame();
    
    GameManager.Instance.StartGame();
}
}