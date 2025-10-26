using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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
    private bool isDead = false;

    public int attackDamage = 10;

    public Slider hpSlider;

    //�״� ȿ��
    public GameObject shardPrefab; // ���� ť�� ������
    public int shardCount = 10;    // ������ ť�� ����
    public float explosionForce = 5f; // ���� ��
    public float explosionRadius = 2f; // ���� ����

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

        // ���� ���� ���� ���� �̵��� ���� (�߷��� ��� �����)
        if (isAttacking)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            animator.SetFloat("speed", 0); // �ӵ��� 0���� ����
            return;
        }

        // --- �̵� ó�� ---
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 targetVelocity = direction * moveSpeed;

        // Y�� �ӵ��� ���� Rigidbody�� ���� �״�� ��� (������ �߷� ������ ����)
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);

        // --- ȸ�� ó�� ---
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0;

        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }

        // --- �ִϸ����� ���� ---
        // ���� �ӷ¸� ����Ͽ� �ִϸ����Ϳ� ����
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        animator.SetFloat("speed", horizontalVelocity.magnitude);
    }

    //  CharacterController�� �浹 ������ (Trigger)
    private void OnTriggerStay(Collider other)
    {
        // �÷��̾ ���� ���� �ȿ� �ӹ��� �ִ� ����
        if (other.CompareTag("Player"))
        {
            // ���� ��Ÿ���� �����ٸ� �ٽ� ����
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time; // ���� ������ ��Ÿ�� �ʱ�ȭ
                StartCoroutine(AttackRoutine());
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
        if (isDead) return; // �̹� ���� ���̸� ����


        currentHP -= damage;
        hpSlider.value = (float)currentHP / maxHP;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return; // Ȥ�ó� �ٽ� �Ҹ� ��� ����
        isDead = true;

        Vector3 explosionCenter = transform.position + Vector3.up * 1f;

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
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.AddKill();
        }

        Destroy(gameObject);
    }


}