using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    //privata
    private PlayerData playerData;

    //public

    //function
    public void Start()
    {
        SetPlayerData();
    }

    private void SetPlayerData()
    {
        playerData = PlayerDataManager.Instance.GetData();
    }

    public void UseHealth(int amount)
    {
        playerData.Hp -= amount;
        //Debug.Log("Player Hp : " + playerData.Hp);

        if (playerData.Hp <= 0)
        {
            CSceneManager.Instance.LoadScene("End_Scene");
        }
    }
}