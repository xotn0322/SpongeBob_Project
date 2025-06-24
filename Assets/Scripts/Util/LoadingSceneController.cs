using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using TMPro;

public class LoadingSceneController : MonoBehaviour
{
    public static LoadingSceneController Instance
    {
        get
        {
            return _instance;
        }
    }
    private static LoadingSceneController _instance;

    public Image ProgressBar;
    public TextMeshProUGUI ProgressText;

    public void Awake()
    {
        _instance = this;
    }

    public void SetLoadingProgress(float amount)
    {
        ProgressBar.fillAmount = amount;
    }

    public void SetLoadingText(string text)
    {
        ProgressText.text = text;
    }
}
