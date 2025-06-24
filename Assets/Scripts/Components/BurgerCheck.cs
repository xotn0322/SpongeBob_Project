using UnityEngine;
using UnityEngine.Animations;

public class BurgerCheck : MonoBehaviour
{
    public EnemyComponent parent;

    void OnTriggerEnter(Collider other)
    {
        parent.OnChildTriggerEnter(other);
    }
}