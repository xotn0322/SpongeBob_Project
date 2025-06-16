using System.Collections.Generic;
using UnityEngine;

public class EnemyGanerator : MonoBehaviour
{
    //private
    private List<Vector3> enemyPositions = new List<Vector3>();

    //public


    //function
    public void Init()
    {
        PositionSetup();
    }

    private void PositionSetup()
    {
        enemyPositions.Add(new Vector3(0, 0, 0));
    }
    
}