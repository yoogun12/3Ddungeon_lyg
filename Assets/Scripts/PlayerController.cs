using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

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

    public Slider hpSlider;

    // --- 추가: 사망 및 UI 페이드 인 관련 변수 ---
    [Header("UI Settings")]
    public CanvasGroup gameOverPanel; // 1단계에서 만든 패널의 CanvasGroup
    public float fadeDuration = 1.5f; // 페이드 인에 걸리는 시간 (초)

    private bool isDead = false; // 사망 상태 플래그
    // ------------------------------------------

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        pov = virtualCam.GetCinemachineComponent<CinemachinePOV>();

        currentHP = maxHP;

        // --- 추가: 게임 시작 시 패널 숨기기 ---
        if (gameOverPanel != null)
        {
            gameOverPanel.alpha = 0f;
            gameOverPanel.interactable = false;
            gameOverPanel.blocksRaycasts = false;
        }
        // ------------------------------------

        hpSlider.value = 1f;
    }

    void Update()
    {
        // --- 추가: 사망 시 Update 로직 중지 ---
        if (isDead) return;
        // ------------------------------------

        // 1. 땅에 있는지 확인하고 중력/점프에 의한 수직 속도 계산
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = jumpPower;
            animator.SetTrigger("jumpTrigger");
        }

        velocity.y += gravity * Time.deltaTime;

        // 2. 키보드 입력 받기
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 3. 카메라 방향 기준으로 실제 이동 방향 계산
        Vector3 camForward = virtualCam.transform.forward;
        Vector3 camRight = virtualCam.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = camForward * z + camRight * x;

        // 4. 계산된 모든 방향(수평, 수직)을 합쳐서 캐릭터를 한번에 이동
        controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        controller.Move(velocity * Time.deltaTime);

        // 애니메이터 파라미터 업데이트
        animator.SetFloat("Speed", moveDirection.magnitude);
        animator.SetBool("isGrounded", isGrounded);

        // 5. 캐릭터 회전 처리 (카메라 방향 보도록)
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
        // --- 추가: 이미 죽었다면 데미지 받지 않음 ---
        if (isDead) return;
        // ----------------------------------------

        currentHP -= damage;
        hpSlider.value = (float)currentHP / maxHP;

        if (currentHP <= 0)
        {
            currentHP = 0; // 체력이 마이너스로 내려가지 않게
            Die();
        }
    }

    // --- 수정: Die() 함수 내용 채우기 ---
    void Die()
    {
        // 중복 사망 방지
        if (isDead) return;
        isDead = true;

        Debug.Log("플레이어가 사망했습니다.");

        // TODO: "DieTrigger" 같은 사망 애니메이션 트리거를 설정했다면 여기서 호출
        // animator.SetTrigger("DieTrigger"); 

        // 플레이어 컨트롤러 스크립트를 비활성화하여 Update()가 멈추고 조작이 불가능하게 함
        this.enabled = false;

        // --- 페이드 인 코루틴 시작 ---
        if (gameOverPanel != null)
        {
            StartCoroutine(FadeInGameOverPanel());
        }
        // ------------------------------
    }

    // --- 추가: 페이드 인 코루틴 ---
    private IEnumerator FadeInGameOverPanel()
    {
        float timer = 0f;

        // 패널을 켜되, 아직 알파는 0
        // (Start에서 이미 켜뒀지만, 만약을 위해)
        gameOverPanel.gameObject.SetActive(true);

        // fadeDuration(예: 1.5초) 동안 반복
        while (timer < fadeDuration)
        {
            // Lerp(A, B, t): A에서 B까지 t(0.0~1.0) 비율로 값을 계산
            // (timer / fadeDuration) 값이 0에서 1로 서서히 증가함
            float newAlpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            gameOverPanel.alpha = newAlpha;

            timer += Time.deltaTime; // 시간 증가
            yield return null; // 다음 프레임까지 대기
        }

        // 루프가 끝나면 알파 값을 1로 확정
        gameOverPanel.alpha = 1f;

        // 이제 패널이 완전히 보이고 클릭(상호작용)도 가능하게 설정
        gameOverPanel.interactable = true;
        gameOverPanel.blocksRaycasts = true;
    }
    // --------------------------------
}