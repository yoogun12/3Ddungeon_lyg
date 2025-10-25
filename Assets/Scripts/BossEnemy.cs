using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    public float attackRange = 10f;
    public float attackCooldown = 2f;
    private Transform player;
    public Animator animator;

    // --- 원거리 공격용 변수 ---
    public GameObject projectilePrefab; // 발사할 프리팹
    public Transform[] firePoints; // 12개의 발사 위치

    // --- 추가: 발사 간격 제어 변수 ---
    [Header("Random Firing Settings")]
    public float minTimeBetweenShots = 0.05f; // 최소 발사 간격 (초)
    public float maxTimeBetweenShots = 0.2f;  // 최대 발사 간격 (초)
    // ------------------------------------

    private float lastAttackTime = 0f;
    private bool isAttacking = false;

    public int maxHP = 5;
    private int currentHP;

    // 죽는 효과
    public GameObject shardPrefab;
    public int shardCount = 10;
    public float explosionForce = 5f;
    public float explosionRadius = 2f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHP = maxHP;
    }

    void Update()
    {
        if (player == null || isAttacking)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    // --- AttackRoutine() 수정 ---
    // 12발을 한 번에 발사하는 대신, 루프 안에서 랜덤 딜레이를 줍니다.
    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        animator.SetTrigger("attack");

        Debug.Log("플레이어에게 12연발 랜덤 발사!");

        // TODO: 애니메이션 '선딜레이' (발사 직전까지의 대기 시간)
        // (예: 0.5초)
        yield return new WaitForSeconds(0.5f);

        // --- 12개 랜덤 발사 로직 ---
        if (projectilePrefab != null && firePoints != null && firePoints.Length > 0)
        {
            // 12개의 포탑을 순회하며
            foreach (Transform point in firePoints)
            {
                if (point == null) continue;

                // 1. 발사체 생성
                Instantiate(projectilePrefab, point.position, point.rotation);

                // 2. 다음 발사까지 랜덤 시간 대기
                float randomDelay = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
                yield return new WaitForSeconds(randomDelay);
            }
        }

        // --- 삭제 ---
        // 모든 발사가 끝난 후의 대기 시간(yield return new WaitForSeconds(1f - 0.5f);)을 삭제했습니다.
        // 왜냐하면 이제 발사 자체가 시간을 차지하기 때문입니다.
        // (필요하다면 여기에 애니메이션 후딜레이를 위한 짧은 대기시간을 추가할 수 있습니다.)

        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Vector3 explosionCenter = transform.position + Vector3.up * 1f;

        for (int i = 0; i < shardCount; i++)
        {
            Vector3 spawnPos = explosionCenter + Random.insideUnitSphere * 0.1f;
            GameObject shard = Instantiate(shardPrefab, spawnPos, Random.rotation);
            Rigidbody rb = shard.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, explosionCenter, explosionRadius);
            }
            Destroy(shard, 2f);
        }
        Destroy(gameObject);
    }
}