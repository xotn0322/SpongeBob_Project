using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    public EEnemyName enemyName;

    //private
    private EnemyData enemyData;

    //functions
    public void Init()
    {
        SetEnemyData();
    }

    private void SetEnemyData()
    {
        enemyData = EnemyDataManager.Instance.GetData(enemyName);
    }

    public EnemyData GetEnemyData()
    {
        return enemyData;
    }
}