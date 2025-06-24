using UnityEngine;

public class SceneChange : MonoBehaviour
{
public void btnSceneChange()
{
    CSceneManager.Instance.LoadSceneWithProcess("SpongeBob_Project");
}

public void QuitGame()
{
    GameManager.Instance.QuitGame();
}
}