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

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null || isAttacking) return;

        // 플레이어를 향해 이동
        Vector3 targetPosition = player.position;
        targetPosition.y = transform.position.y;

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
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
}