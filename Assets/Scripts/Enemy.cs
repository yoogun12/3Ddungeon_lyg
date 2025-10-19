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

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        // �÷��̾ ���� �̵�
        Vector3 targetPosition = player.position;
        targetPosition.y = transform.position.y;

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
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
}