using UnityEngine;
using System.Collections;

public class HomingProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float homingDuration = 0.8f;
    public float turnSpeed = 6f;

    // --- �߰�: �̻��� ������ ���� ---
    [Header("Damage Settings")]
    public int damageAmount = 10; // �̻����� �÷��̾�� �� ������

    [Header("Explosion Shards Settings")]
    public GameObject[] shardPrefabs; // 2���� ���� �׸� ���� ������
    public int minShardCount = 5;
    public int maxShardCount = 10;
    public float shardExplosionForce = 10f;
    public float shardExplosionRadius = 1f;
    public float shardDestroyTime = 2f;

    private Transform playerTarget;
    private Rigidbody rb;
    private float timeSpawned;
    private bool isHoming = true;

    // �÷��̾��� Collider�� ������ ����
    private Collider playerCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false; // �߷� ��Ȱ��ȭ
            rb.freezeRotation = true; // ���� ������ ���� ���� ȸ�� ����
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;

            // �÷��̾��� Collider(CharacterController)�� ã�� ����
            playerCollider = playerObj.GetComponent<Collider>();
        }

        timeSpawned = Time.time;

        rb.velocity = transform.forward * speed;

        Destroy(gameObject, 5f); // 5�� �� �ڵ� �ı�
    }

    void FixedUpdate()
    {
        // 1. ���� �ð� üũ
        if (isHoming && (Time.time > timeSpawned + homingDuration))
        {
            isHoming = false;
        }

        // 2. ���� ���� (isHoming�� true�� ����)
        if (playerTarget != null && isHoming)
        {
            Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            rb.velocity = transform.forward * speed;
        }

        // isHoming�� false�� �Ǹ� ������ ���߰� �߷� ���� ���� ����
    }

    // --- �浹 ���� (Trigger ���) ---
    private void OnTriggerEnter(Collider other)
    {
        HandleImpact(other.gameObject);
    }

    // --- �浹 ���� (Collision ���) ---
    private void OnCollisionEnter(Collision collision)
    {
        HandleImpact(collision.gameObject);
    }

    // --- �浹 ó�� �Լ� (������ ���� ����) ---
    void HandleImpact(GameObject collidedObject)
    {
        // 1. �÷��̾�� �ε������� Ȯ��
        if (collidedObject.CompareTag("Player"))
        {
            // 2. �÷��̾��� PlayerController ��ũ��Ʈ ��������
            PlayerController player = collidedObject.GetComponent<PlayerController>();
            if (player != null)
            {
                // 3. ������ �ֱ�!
                player.TakeDamage(damageAmount);
            }

            // 4. ���� �� �̻��� �ı�
            ExplodeWithShards();
        }
        // 5. ���� �ε������� Ȯ��
        else if (collidedObject.CompareTag("Ground"))
        {
            // ������ ���� ���� �� �̻��� �ı�
            ExplodeWithShards();
        }
    }

    // --- ���� ���� �Լ� ---
    void ExplodeWithShards()
    {
        if (shardPrefabs != null && shardPrefabs.Length > 0)
        {
            int actualShardCount = Random.Range(minShardCount, maxShardCount + 1);

            for (int i = 0; i < actualShardCount; i++)
            {
                // 2���� ���� �� ���� ����
                GameObject randomShardPrefab = shardPrefabs[Random.Range(0, shardPrefabs.Length)];
                Vector3 spawnPos = transform.position + Random.insideUnitSphere * 0.1f;
                GameObject shard = Instantiate(randomShardPrefab, spawnPos, Random.rotation);

                // ����� �÷��̾� ���� �浹 ���� ���
                Collider shardCollider = shard.GetComponent<Collider>();
                if (shardCollider != null && playerCollider != null)
                {
                    Physics.IgnoreCollision(shardCollider, playerCollider);
                }

                Rigidbody shardRb = shard.GetComponent<Rigidbody>();
                if (shardRb != null)
                {
                    // ���� �� ���ϱ�
                    shardRb.AddExplosionForce(shardExplosionForce, transform.position, shardExplosionRadius);
                    // ������ �߷� ���� �ް�
                    shardRb.useGravity = true;
                }

                // ���� �ڵ� �ı�
                Destroy(shard, shardDestroyTime);
            }
        }

        // �̻��� �ڽ� �ı�
        Destroy(gameObject);
    }
}