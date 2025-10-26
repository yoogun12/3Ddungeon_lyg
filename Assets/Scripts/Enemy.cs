using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    private bool isDead = false;

    public int attackDamage = 10;

    public Slider hpSlider;

    //죽는 효과
    public GameObject shardPrefab; // 빨간 큐브 프리팹
    public int shardCount = 10;    // 생성할 큐브 개수
    public float explosionForce = 5f; // 폭발 힘
    public float explosionRadius = 2f; // 폭발 범위

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentHP = maxHP;
        hpSlider.value = 1f;
    }

    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (player == null || rb == null) return;

        // 공격 중일 때는 수평 이동을 멈춤 (중력은 계속 적용됨)
        if (isAttacking)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            animator.SetFloat("speed", 0); // 속도도 0으로 설정
            return;
        }

        // --- 이동 처리 ---
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 targetVelocity = direction * moveSpeed;

        // Y축 속도는 현재 Rigidbody의 값을 그대로 사용 (점프나 중력 유지를 위함)
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);

        // --- 회전 처리 ---
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;

        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }

        // --- 애니메이터 설정 ---
        // 수평 속력만 계산하여 애니메이터에 전달
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        animator.SetFloat("speed", horizontalVelocity.magnitude);
    }

    //  CharacterController와 충돌 감지용 (Trigger)
    private void OnTriggerStay(Collider other)
    {
        // 플레이어가 공격 범위 안에 머물러 있는 동안
        if (other.CompareTag("Player"))
        {
            // 공격 쿨타임이 지났다면 다시 공격
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time; // 공격 직전에 쿨타임 초기화
                StartCoroutine(AttackRoutine());
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
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(attackDamage);
            }
        }
        yield return new WaitForSeconds(1f);

        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // 이미 죽은 적이면 무시


        currentHP -= damage;
        hpSlider.value = (float)currentHP / maxHP;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return; // 혹시나 다시 불릴 경우 방지
        isDead = true;

        Vector3 explosionCenter = transform.position + Vector3.up * 1f;

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
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.AddKill();
        }

        Destroy(gameObject);
    }


}