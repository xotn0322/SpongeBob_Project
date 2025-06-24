using UnityEngine;

public class DestinationComponent : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyComponent>(out EnemyComponent enemyComponent))
        {
            if (!enemyComponent.IsDead())
            {
                enemyComponent.AttackPlayer(1);
            }
            
            Destroy(enemyComponent.gameObject);
        }
    }
}