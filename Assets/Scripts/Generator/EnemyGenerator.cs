using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Unity.Profiling;
using System.Runtime.CompilerServices;
using UnityEngine.AI;

public class EnemyGenerator : MonoBehaviour
{
    //private
    private List<Vector3> enemyPositions = new List<Vector3>();
    private BoxCollider boxCollider; // BoxCollider에 대한 참조
    private GameObject prefab;

    //public
    public EEnemyName enemyType; // 스폰할 적의 이름
    public float fixedYPosition = 0f; // 고정된 Y 위치
    public long spawnIntervalMs = 5000; // 5초 기본 스폰 간격 (밀리초)
    public bool prefabRandom = false;
    public GameObject destination;

    //function
    public void Start()
    {
        Init();
    }

    public void Init()
    {
        boxCollider = GetComponent<BoxCollider>(); // Collider 컴포넌트 가져오기
        if (boxCollider == null)
        {
            Debug.LogError("EnemyGenerator requires a BoxCollider component!");
            return;
        }

        // 적 스폰을 위한 타이머 설정
        Timer spawnTimer = new Timer();
        spawnTimer.SetTimer(ETimerType.GameTime, false, originalTimeMs: spawnIntervalMs, actionOnExpire: (t) =>
        {
            SpawnEnemy();
            t.CurrentTimeMs = spawnIntervalMs; // 다음 스폰을 위해 타이머 재설정
        });
        TimeManager.Instance.ResisterTimer(spawnTimer); // 타이머 등록
    }

    private void SpawnEnemy()
    {
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider is missing, cannot spawn enemy.");
            return;
        }

        Bounds bounds = boxCollider.bounds;

        // BoxCollider 범위 내에서 무작위 X와 Z 계산, Y는 고정
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);
        Vector3 spawnPosition = new Vector3(randomX, fixedYPosition, randomZ);

        // EnemyManager.SpawnPrefab()을 사용하여 적 생성
        if (prefabRandom)
        {
            Array enumValues = Enum.GetValues(typeof(EEnemyName));
            EEnemyName randomEnemyName = (EEnemyName)enumValues.GetValue(UnityEngine.Random.Range(0, enumValues.Length));

            prefab = EnemyManager.Instance.SpawnPrefab(randomEnemyName, spawnPosition);
        }
        else
        {
            prefab = EnemyManager.Instance.SpawnPrefab(enemyType, spawnPosition);
        }

        SetDestination(prefab.GetComponent<NavMeshAgent>());
    }

    private void SetDestination(NavMeshAgent agent)
    {
        agent.SetDestination(new Vector3(destination.transform.position.x, prefab.transform.position.y, destination.transform.position.z));
    }
}