using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;          // �̵� �ӵ�
    public float attackRange = 1.5f;      // ���� ���� �Ÿ�
    public float attackCooldown = 2f;     // ���� ��Ÿ��
    private Transform player;             // �÷��̾� ������
    public Animator animator;


    private float lastAttackTime = 0f;
    private bool isAttacking = false;     // ���� �� ����

    public int maxHP = 5;

    private int currentHP;

    //�״� ȿ��
    public GameObject shardPrefab; // ���� ť�� ������
    public int shardCount = 10;    // ������ ť�� ����
    public float explosionForce = 5f; // ���� ��
    public float explosionRadius = 2f; // ���� ����

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

        // Rigidbody�� �̵� ó��
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.MovePosition(transform.position + move);
        }

        transform.LookAt(player.position);
        animator.SetFloat("speed", moveSpeed);
    }

    //  CharacterController�� �浹 ������ (Trigger)
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

        Debug.Log("�÷��̾� ����!");

        // ���� �ִϸ��̼� ���̿� �°� ���
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
            // �� �߾� �ֺ����� ���ݸ� ����
            Vector3 spawnPos = explosionCenter + Random.insideUnitSphere * 0.1f;
            GameObject shard = Instantiate(shardPrefab, spawnPos, Random.rotation);

            Rigidbody rb = shard.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // ���� �߽��� �׻� �� �߾�
                rb.AddExplosionForce(explosionForce, explosionCenter, explosionRadius);
            }

            Destroy(shard, 2f);
        }

        Destroy(gameObject);
    }


}