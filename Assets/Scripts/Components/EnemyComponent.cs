using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyComponent : MonoBehaviour
{
    //private
    private EnemyData enemyData;
    private GameObject Player;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private bool isDead = false;

    //public
    public EEnemyName enemyName;

    //functions
    public void Init()
    {
        // Player 오브젝트를 찾아 할당합니다.
        // Player 오브젝트에 'Player' 태그가 할당되어 있어야 합니다.
        Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            Debug.LogError("Player GameObject with 'Player' tag not found!");
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"Animator component not found on {gameObject.name}!");
        }

        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError($"NavMeshAgent component not found on {gameObject.name}!");
        }

        SetEnemyData();
    }

    private void SetEnemyData()
    {
        enemyData = EnemyDataManager.Instance.GetData(enemyName);
        navMeshAgent.speed = enemyData.Speed;
    }

    public EnemyData GetEnemyData()
    {
        return enemyData;
    }

    public void UseHealth(int amount)
    {
        enemyData.Hp -= amount;
        StartEating();
    }

    public void AttackPlayer(int amount)
    {
        Player.GetComponent<PlayerComponent>().UseHealth(amount);
    }

    public void StartEating()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
        }

        if (animator != null)
        {
            animator.SetTrigger("Eat"); // "Eat"이라는 트리거 파라미터가 Animator에 있다고 가정합니다.
            StartCoroutine(CheckDeathAfterAnimation()); // "Eat" 애니메이션의 이름을 전달합니다.
        }
        else
        {
            // Animator가 없으면 바로 죽음 체크를 진행합니다.
            CheckDeathImmediately();
        }
    }

    private IEnumerator CheckDeathAfterAnimation()
    {
        if (animator == null || !animator.enabled)
        {
            CheckDeathImmediately();
            yield break;
        }

        yield return null;

        do
        {
            yield return null;
        } while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Customer_Standing_Drink_Pint_Chug"));

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        // Eat 애니메이션이 끝나면 animator를 멈춤
        animator.enabled = false;
        // 죽음 체크
        if (enemyData.Hp <= 0 && !isDead)
        {
            isDead = true;
            Debug.Log($"{gameObject.name} has died (immediate after animation).");
            if (navMeshAgent != null) navMeshAgent.enabled = false;
            var rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
            Destroy(gameObject);
            yield break;
        }
        else
        {
            animator.enabled = true;
            Debug.Log($"{gameObject.name} survived eating with {enemyData.Hp} HP remaining.");
            if (navMeshAgent != null)
            {
                navMeshAgent.isStopped = false;
            }
        }
    }

    private void CheckDeathImmediately()
    {
        if (enemyData.Hp <= 0 && !isDead)
        {
            isDead = true;
            Debug.Log($"{gameObject.name} has died.");
            if (navMeshAgent != null) navMeshAgent.enabled = false;
            var rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
            if (animator != null) animator.enabled = false;
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"{gameObject.name} survived eating with {enemyData.Hp} HP remaining.");
            if (navMeshAgent != null)
            {
                navMeshAgent.isStopped = false;
            }
        }
    }

    public void OnChildTriggerEnter(Collider other)
    {
        // 부모의 부모에서 BurgerStackManager를 찾음
        var grandParent = other.transform.parent != null ? other.transform.parent.parent : null;
        if (grandParent != null && grandParent.TryGetComponent<BurgerStackManager>(out BurgerStackManager burgerStackManager))
        {
            UseHealth(burgerStackManager.GetTotalDamage());
            // 햄버거도 같이 파괴
            Destroy(burgerStackManager.gameObject);
        }
    }

    private void OnDestroy()
    {
        // Enemy가 파괴될 때 EnemyManager에서 해당 Enemy를 제거합니다.
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.RemoveEnemy(gameObject.GetInstanceID());
        }
    }

    public bool IsDead()
    {
        return isDead;
    }
}