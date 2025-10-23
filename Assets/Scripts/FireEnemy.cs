using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEnemy : MonoBehaviour
{
  
    public float moveSpeed = 2f;
    public float attackRange = 10f; // 원거리 공격을 위해 사거리를 늘려줍니다. (Increased for ranged)
    public float attackCooldown = 2f;
    private Transform player;
    public Animator animator;

    // --- 원거리 공격용 변수 추가 (Added variables for ranged attack) ---
    public GameObject projectilePrefab; // 발사할 프리팹
    public Transform firePoint;         // 발사 위치 (총구, 손 등)
    public float projectileSpeed = 15f; // 발사체 속도

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

    // 변경점: Update()를 사용하여 거리 체크 및 공격/이동 로직을 관리합니다.
    void Update()
    {
        if (player == null || isAttacking)
        {
            // 플레이어가 없거나 공격 중일 때는 아무것도 하지 않음
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 플레이어가 공격 범위 안에 있고, 쿨타임이 지났다면 공격
        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(AttackRoutine());
        }
        // 플레이어가 공격 범위 밖에 있다면 추적
        else if (distanceToPlayer > attackRange)
        {
            // 이동 중일 때만 회전하도록 처리
            Vector3 lookDir = player.position - transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }

    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (player == null || rb == null) return;

        // 공격 중이거나 플레이어가 공격 범위 안에 있으면 수평 이동을 멈춤
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (isAttacking || distanceToPlayer <= attackRange)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            animator.SetFloat("speed", 0);
            return;
        }

        // --- 이동 처리 ---
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 targetVelocity = direction * moveSpeed;
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);

        // --- 애니메이터 설정 ---
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        animator.SetFloat("speed", horizontalVelocity.magnitude);
    }

    // 삭제: OnTriggerStay는 이제 필요 없습니다.
    // private void OnTriggerStay(Collider other) { ... }

    // 변경점: 발사체를 생성하고 발사하는 로직으로 수정
    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time; // 공격 시작 시 쿨타임 초기화

        // 플레이어 방향으로 즉시 회전
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);

        // 애니메이션 실행
        animator.SetTrigger("attack");
        animator.SetFloat("speed", 0f);

        Debug.Log("플레이어에게 원거리 공격!");

        // TODO: 여기서 애니메이션에 맞춰 발사 타이밍을 조절할 수 있습니다.
        // 예를 들어, 0.5초 뒤에 발사하고 싶다면 아래 라인을 추가하세요.
        yield return new WaitForSeconds(0.5f);

        // --- 발사체 생성 및 발사 ---
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // firePoint의 정면 방향으로 발사
                rb.velocity = firePoint.forward * projectileSpeed;
            }
            // 발사체는 일정 시간 뒤에 파괴
            Destroy(projectile, 5f);
        }

        // 공격 애니메이션 전체 길이에 맞게 대기 (예: 1초)
        yield return new WaitForSeconds(1f - 0.5f); // 위에서 0.5초 기다렸으므로 나머지 시간 대기

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

