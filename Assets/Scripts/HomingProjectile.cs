using UnityEngine;
using System.Collections;

public class HomingProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float homingDuration = 0.8f;
    public float turnSpeed = 6f;

    // --- 추가: 미사일 데미지 변수 ---
    [Header("Damage Settings")]
    public int damageAmount = 10; // 미사일이 플레이어에게 줄 데미지

    [Header("Explosion Shards Settings")]
    public GameObject[] shardPrefabs; // 2가지 색상 네모 파편 프리팹
    public int minShardCount = 5;
    public int maxShardCount = 10;
    public float shardExplosionForce = 10f;
    public float shardExplosionRadius = 1f;
    public float shardDestroyTime = 2f;

    private Transform playerTarget;
    private Rigidbody rb;
    private float timeSpawned;
    private bool isHoming = true;

    // 플레이어의 Collider를 저장할 변수
    private Collider playerCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false; // 중력 비활성화
            rb.freezeRotation = true; // 물리 엔진에 의한 임의 회전 방지
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;

            // 플레이어의 Collider(CharacterController)를 찾아 저장
            playerCollider = playerObj.GetComponent<Collider>();
        }

        timeSpawned = Time.time;

        rb.velocity = transform.forward * speed;

        Destroy(gameObject, 5f); // 5초 뒤 자동 파괴
    }

    void FixedUpdate()
    {
        // 1. 유도 시간 체크
        if (isHoming && (Time.time > timeSpawned + homingDuration))
        {
            isHoming = false;
        }

        // 2. 유도 로직 (isHoming이 true일 때만)
        if (playerTarget != null && isHoming)
        {
            Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            rb.velocity = transform.forward * speed;
        }

        // isHoming이 false가 되면 유도를 멈추고 중력 없이 직선 비행
    }

    // --- 충돌 감지 (Trigger 방식) ---
    private void OnTriggerEnter(Collider other)
    {
        HandleImpact(other.gameObject);
    }

    // --- 충돌 감지 (Collision 방식) ---
    private void OnCollisionEnter(Collision collision)
    {
        HandleImpact(collision.gameObject);
    }

    // --- 충돌 처리 함수 (데미지 로직 포함) ---
    void HandleImpact(GameObject collidedObject)
    {
        // 1. 플레이어와 부딪혔는지 확인
        if (collidedObject.CompareTag("Player"))
        {
            // 2. 플레이어의 PlayerController 스크립트 가져오기
            PlayerController player = collidedObject.GetComponent<PlayerController>();
            if (player != null)
            {
                // 3. 데미지 주기!
                player.TakeDamage(damageAmount);
            }

            // 4. 폭발 및 미사일 파괴
            ExplodeWithShards();
        }
        // 5. 땅과 부딪혔는지 확인
        else if (collidedObject.CompareTag("Ground"))
        {
            // 데미지 없이 폭발 및 미사일 파괴
            ExplodeWithShards();
        }
    }

    // --- 파편 폭발 함수 ---
    void ExplodeWithShards()
    {
        if (shardPrefabs != null && shardPrefabs.Length > 0)
        {
            int actualShardCount = Random.Range(minShardCount, maxShardCount + 1);

            for (int i = 0; i < actualShardCount; i++)
            {
                // 2가지 색상 중 랜덤 선택
                GameObject randomShardPrefab = shardPrefabs[Random.Range(0, shardPrefabs.Length)];
                Vector3 spawnPos = transform.position + Random.insideUnitSphere * 0.1f;
                GameObject shard = Instantiate(randomShardPrefab, spawnPos, Random.rotation);

                // 파편과 플레이어 간의 충돌 무시 명령
                Collider shardCollider = shard.GetComponent<Collider>();
                if (shardCollider != null && playerCollider != null)
                {
                    Physics.IgnoreCollision(shardCollider, playerCollider);
                }

                Rigidbody shardRb = shard.GetComponent<Rigidbody>();
                if (shardRb != null)
                {
                    // 폭발 힘 가하기
                    shardRb.AddExplosionForce(shardExplosionForce, transform.position, shardExplosionRadius);
                    // 파편은 중력 영향 받게
                    shardRb.useGravity = true;
                }

                // 파편 자동 파괴
                Destroy(shard, shardDestroyTime);
            }
        }

        // 미사일 자신 파괴
        Destroy(gameObject);
    }
}