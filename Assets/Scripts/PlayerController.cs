using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    public float jumpPower = 5f;

    public float gravity = -9.81f;

    private CharacterController controller;

    private Vector3 velocity;

    public bool isGrounded;

    private Animator animator;

    public CinemachineVirtualCamera virtualCam;

    public float rotationSpeed = 10f;

    private CinemachinePOV pov;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        pov = virtualCam.GetCinemachineComponent<CinemachinePOV>();
        //virtual Camera의 POV 컴포넌트 가져오기
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = controller.isGrounded;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(x, 0, z);
        controller.Move(move * speed * Time.deltaTime);

        animator.SetFloat("Speed", move.magnitude);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = jumpPower;
            animator.SetTrigger("jumpTrigger");
        }

        animator.SetBool("isGrounded", isGrounded);


        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        //카메라 기준 방향 계산
        Vector3 camForward = virtualCam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = virtualCam.transform.right;
        camRight.Normalize();

        Vector3 cmove = (camForward * z + camRight * x).normalized; // 이동 방향 = 카메라 forward/right 기반
        controller.Move(cmove * speed * Time.deltaTime);

        float cameraYaw = pov.m_HorizontalAxis.Value; // 마우스 좌우 회전값
        Quaternion targetRot = Quaternion.Euler(0f, cameraYaw, 0f); 
        transform.rotation = Quaternion.Slerp(transform.rotation,targetRot,rotationSpeed * Time.deltaTime);

        //카메라 중앙쪽으로 보게하는 코드
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            pov.m_HorizontalAxis.Value = transform.eulerAngles.y;
            pov.m_VerticalAxis.Value = 0f;
        }
    }
}