using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CSceneManager : MonoBehaviour
{
    public static CSceneManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private static CSceneManager _instance;

    private static string _nextScene;


    public void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);
    }

    public void LoadSceneWithProcess(string sceneName)
    {
        _nextScene = sceneName;
        //SceneManager.LoadScene("LoadingScene");
        StartCoroutine(LoadSceneProcess());
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadSceneProcess()
    {
        // Wait 2 frames for LoadingScene
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        AsyncOperation op = SceneManager.LoadSceneAsync(_nextScene);
        op.allowSceneActivation = false;

        GameManager.Instance.LoadGame();

        while (GameManager.Instance.IsLoadComplete() == false &&
               op.isDone == false)
        {
            yield return new WaitForEndOfFrame();
            // LoadingSceneController.Instance.SetLoadingProgress(GameManager.Instance.GetLoadProgress());
            // LoadingSceneController.Instance.SetLoadingText(GameManager.Instance.GetLoadProgressText());
        }

        op.allowSceneActivation = true;

        // Wait 2 frames for applying new Scene
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        GameManager.Instance.InitMonoBehaviourGameEngine();

        // Wait 1 frame for MonoBehaviour GameEngine Load
        yield return new WaitForEndOfFrame();
        
        GameManager.Instance.StartGame();
    }
}
