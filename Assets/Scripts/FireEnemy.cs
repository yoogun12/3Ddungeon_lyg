using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEnemy : MonoBehaviour
{
  
    public float moveSpeed = 2f;
    public float attackRange = 10f; // ���Ÿ� ������ ���� ��Ÿ��� �÷��ݴϴ�. (Increased for ranged)
    public float attackCooldown = 2f;
    private Transform player;
    public Animator animator;

    // --- ���Ÿ� ���ݿ� ���� �߰� (Added variables for ranged attack) ---
    public GameObject projectilePrefab; // �߻��� ������
    public Transform firePoint;         // �߻� ��ġ (�ѱ�, �� ��)
    public float projectileSpeed = 15f; // �߻�ü �ӵ�

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

    // ������: Update()�� ����Ͽ� �Ÿ� üũ �� ����/�̵� ������ �����մϴ�.
    void Update()
    {
        if (player == null || isAttacking)
        {
            // �÷��̾ ���ų� ���� ���� ���� �ƹ��͵� ���� ����
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // �÷��̾ ���� ���� �ȿ� �ְ�, ��Ÿ���� �����ٸ� ����
        if (distanceToPlayer <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(AttackRoutine());
        }
        // �÷��̾ ���� ���� �ۿ� �ִٸ� ����
        else if (distanceToPlayer > attackRange)
        {
            // �̵� ���� ���� ȸ���ϵ��� ó��
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

        // ���� ���̰ų� �÷��̾ ���� ���� �ȿ� ������ ���� �̵��� ����
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (isAttacking || distanceToPlayer <= attackRange)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            animator.SetFloat("speed", 0);
            return;
        }

        // --- �̵� ó�� ---
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 targetVelocity = direction * moveSpeed;
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);

        // --- �ִϸ����� ���� ---
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        animator.SetFloat("speed", horizontalVelocity.magnitude);
    }

    // ����: OnTriggerStay�� ���� �ʿ� �����ϴ�.
    // private void OnTriggerStay(Collider other) { ... }

    // ������: �߻�ü�� �����ϰ� �߻��ϴ� �������� ����
    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time; // ���� ���� �� ��Ÿ�� �ʱ�ȭ

        // �÷��̾� �������� ��� ȸ��
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);

        // �ִϸ��̼� ����
        animator.SetTrigger("attack");
        animator.SetFloat("speed", 0f);

        Debug.Log("�÷��̾�� ���Ÿ� ����!");

        // TODO: ���⼭ �ִϸ��̼ǿ� ���� �߻� Ÿ�̹��� ������ �� �ֽ��ϴ�.
        // ���� ���, 0.5�� �ڿ� �߻��ϰ� �ʹٸ� �Ʒ� ������ �߰��ϼ���.
        yield return new WaitForSeconds(0.5f);

        // --- �߻�ü ���� �� �߻� ---
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // firePoint�� ���� �������� �߻�
                rb.velocity = firePoint.forward * projectileSpeed;
            }
            // �߻�ü�� ���� �ð� �ڿ� �ı�
            Destroy(projectile, 5f);
        }

        // ���� �ִϸ��̼� ��ü ���̿� �°� ��� (��: 1��)
        yield return new WaitForSeconds(1f - 0.5f); // ������ 0.5�� ��ٷ����Ƿ� ������ �ð� ���

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

