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
        // 애니메이터가 활성화되어 있지 않으면 바로 다음 프레임으로 넘어갑니다.
        if (animator == null || !animator.enabled)
        {
            CheckDeathImmediately();
            yield break;
        }

        // 애니메이션이 시작될 때까지 기다립니다.
        yield return null;

        // "Customer_Standing_Drink_Pint_Chug" 애니메이션 상태가 될 때까지 기다립니다.
        do
        {
            yield return null;
        } while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Customer_Standing_Drink_Pint_Chug"));

        // 애니메이션이 끝날 때까지 기다립니다.
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        // 애니메이션이 끝난 후 추가적인 딜레이가 필요하면 여기에 추가합니다.
        // yield return new WaitForSeconds(0.5f);

        CheckDeathImmediately();
    }

    private void CheckDeathImmediately()
    {
        if (enemyData.Hp <= 0)
        {
            Debug.Log($"{gameObject.name} has died.");
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
        if (other.TryGetComponent<BurgerStackManager>(out BurgerStackManager burgerStackManager))
        {
            UseHealth(burgerStackManager.GetTotalDamage());
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
}