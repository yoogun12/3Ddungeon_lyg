using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;          // 이동 속도
    public float attackRange = 1.5f;      // 공격 가능 거리
    public float attackCooldown = 2f;     // 공격 쿨타임
    private Transform player;             // 플레이어 추적용
    public Animator animator;


    private float lastAttackTime = 0f;
    private bool isAttacking = false;     // 공격 중 여부

    public int maxHP = 5;

    private int currentHP;

    //죽는 효과
    public GameObject shardPrefab; // 빨간 큐브 프리팹
    public int shardCount = 10;    // 생성할 큐브 개수
    public float explosionForce = 5f; // 폭발 힘
    public float explosionRadius = 2f; // 폭발 범위

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHP = maxHP;
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        Vector3 targetPosition = player.position;
        targetPosition.y = transform.position.y;

        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 move = direction * moveSpeed * Time.deltaTime;

        // Rigidbody로 이동 처리
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.MovePosition(transform.position + move);
        }

        transform.LookAt(player.position);
        animator.SetFloat("speed", moveSpeed);
    }

    //  CharacterController와 충돌 감지용 (Trigger)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                StartCoroutine(AttackRoutine());
                lastAttackTime = Time.time;
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetTrigger("attack");
        animator.SetFloat("speed", 0f);

        Debug.Log("플레이어 공격!");

        // 공격 애니메이션 길이에 맞게 대기
        yield return new WaitForSeconds(1f);

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
        Vector3 explosionCenter = transform.position + Vector3.up * 2f;

        for (int i = 0; i < shardCount; i++)
        {
            // 적 중앙 주변에서 조금만 랜덤
            Vector3 spawnPos = explosionCenter + Random.insideUnitSphere * 0.1f;
            GameObject shard = Instantiate(shardPrefab, spawnPos, Random.rotation);

            Rigidbody rb = shard.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 폭발 중심은 항상 적 중앙
                rb.AddExplosionForce(explosionForce, explosionCenter, explosionRadius);
            }

            Destroy(shard, 2f);
        }

        Destroy(gameObject);
    }


}