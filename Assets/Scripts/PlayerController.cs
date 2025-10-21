using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpPower = 5f;
    public float gravity = -9.81f;
    public float rotationSpeed = 10f;

    public CinemachineVirtualCamera virtualCam;

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;
    private bool isGrounded;
    private CinemachinePOV pov;

    public int maxHP = 100;
    private int currentHP;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        pov = virtualCam.GetCinemachineComponent<CinemachinePOV>();

        currentHP = maxHP;
    }

    void Update()
    {
        // 1. 땅에 있는지 확인하고 중력/점프에 의한 수직 속도 계산
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 땅에 붙어있도록 살짝 아래로 힘을 줌
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = jumpPower;
            animator.SetTrigger("jumpTrigger");
        }

        velocity.y += gravity * Time.deltaTime; // 중력 적용

        // 2. 키보드 입력 받기
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 3. 카메라 방향 기준으로 실제 이동 방향 계산
        Vector3 camForward = virtualCam.transform.forward;
        Vector3 camRight = virtualCam.transform.right;
        camForward.y = 0; // 높낮이는 무시
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = camForward * z + camRight * x;

        // 4. 계산된 모든 방향(수평, 수직)을 합쳐서 캐릭터를 한번에 이동
        controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime); // 수직 이동 적용

        // 애니메이터 파라미터 업데이트
        animator.SetFloat("Speed", moveDirection.magnitude);
        animator.SetBool("isGrounded", isGrounded);

        // 5. 캐릭터 회전 처리 (카메라 방향 보도록)
        // 이동 입력이 있을 때만 캐릭터를 회전시킵니다.
        if (moveDirection != Vector3.zero)
        {
            float cameraYaw = virtualCam.transform.eulerAngles.y;
            Quaternion targetRot = Quaternion.Euler(0f, cameraYaw, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // 6. Tab 키로 카메라 시점 초기화
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            pov.m_HorizontalAxis.Value = transform.eulerAngles.y;
            pov.m_VerticalAxis.Value = 0f;
        }
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
        //플레이어 죽었을때 유다이 뜨게 만들기.
    }
}