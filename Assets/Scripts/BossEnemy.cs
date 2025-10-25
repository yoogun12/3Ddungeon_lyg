using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour
{
    public float attackRange = 10f;
    public float attackCooldown = 2f;
    private Transform player;
    public Animator animator;

    // --- ���Ÿ� ���ݿ� ���� ---
    public GameObject projectilePrefab; // �߻��� ������
    public Transform[] firePoints; // 12���� �߻� ��ġ

    // --- �߰�: �߻� ���� ���� ���� ---
    [Header("Random Firing Settings")]
    public float minTimeBetweenShots = 0.05f; // �ּ� �߻� ���� (��)
    public float maxTimeBetweenShots = 0.2f;  // �ִ� �߻� ���� (��)
    // ------------------------------------

    private float lastAttackTime = 0f;
    private bool isAttacking = false;

    public int maxHP = 5;
    private int currentHP;

    // �״� ȿ��
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

    // --- AttackRoutine() ���� ---
    // 12���� �� ���� �߻��ϴ� ���, ���� �ȿ��� ���� �����̸� �ݴϴ�.
    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        animator.SetTrigger("attack");

        Debug.Log("�÷��̾�� 12���� ���� �߻�!");

        // TODO: �ִϸ��̼� '��������' (�߻� ���������� ��� �ð�)
        // (��: 0.5��)
        yield return new WaitForSeconds(0.5f);

        // --- 12�� ���� �߻� ���� ---
        if (projectilePrefab != null && firePoints != null && firePoints.Length > 0)
        {
            // 12���� ��ž�� ��ȸ�ϸ�
            foreach (Transform point in firePoints)
            {
                if (point == null) continue;

                // 1. �߻�ü ����
                Instantiate(projectilePrefab, point.position, point.rotation);

                // 2. ���� �߻���� ���� �ð� ���
                float randomDelay = Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
                yield return new WaitForSeconds(randomDelay);
            }
        }

        // --- ���� ---
        // ��� �߻簡 ���� ���� ��� �ð�(yield return new WaitForSeconds(1f - 0.5f);)�� �����߽��ϴ�.
        // �ֳ��ϸ� ���� �߻� ��ü�� �ð��� �����ϱ� �����Դϴ�.
        // (�ʿ��ϴٸ� ���⿡ �ִϸ��̼� �ĵ����̸� ���� ª�� ���ð��� �߰��� �� �ֽ��ϴ�.)

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