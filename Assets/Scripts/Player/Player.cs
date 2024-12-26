using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    [SerializeField] private float jumpForce;
    private Rigidbody2D rb;
    private bool isGrounded = true;
    private Animator animator;
    private CapsuleCollider2D capsuleCollider2D;

    [SerializeField] private int currentLives = 3; // 실제 목숨 값 저장

    private bool isInvincible =false;

    // 피격 이펙트 시각화를 위한 필드들
    private SpriteRenderer spriteRenderer;            // 스프라이트 렌더러 리스트

    // 새 Input System에서 자동 생성되는 클래스(네이밍 규칙에 따라 변경 가능)
    private PlayerInputActions playerInputActions;
    // 체력 변경 이벤트
    public event System.Action<float> OnHealthChanged;

    private void Awake()
    {
        // InputActions 인스턴스 생성
        rb= GetComponent<Rigidbody2D>();
        playerInputActions = new PlayerInputActions();
        animator = GetComponent<Animator>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public int CurrentLives
    {
        get => currentLives;
        set
        {
            if (currentLives != value) // 목숨이 변경될 때만 실행
            {
                currentLives = Mathf.Clamp(value, 0, 3); // 값 범위 제한
                OnHealthChanged?.Invoke(currentLives); // 이벤트 호출
            }
        }
    }



    // 점프 콜백함수
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            animator.SetInteger("state", 1);
            Jump();
            isGrounded = false;
        }
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        
    }

    // 실제 점프 동작
    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        Managers.Audio.PlaySound("Jump");
    }

    // Ground와 충돌했을 때 땅으로 인식
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name=="Ground")
        {
            if(!isGrounded)
            {
                animator.SetInteger("state", 2);
            }
            isGrounded = true;
        }
    }

    /// <summary>
    /// 데미지를 받을 때 시각화
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlashRed()
    {
        isInvincible = true; // 무적 상태 시작

        for (int i = 0; i < 3; i++) // 세 차례에 걸쳐 깜박임
        {
            // 모든 스프라이트 색상을 빨간색으로 변경
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f); // 0.1초 대기

            // 모든 스프라이트 색상을 원래 색상으로 복원
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f); // 0.1초 대기
        }

        isInvincible = false; // 무적 상태 종료
    }

    void KillPlayer()
    {
        capsuleCollider2D.enabled = false;
        animator.enabled=false;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        StartCoroutine(GameOver());
    }

    private IEnumerator GameOver()
    {
        Managers.Audio.PlaySound("GameOver");
        yield return new WaitForSeconds(3f); // 2초 대기
        Managers.Game.SaveHighScore();
        Managers.UI.ShowPopupUI<UI_GameOver>();
        DebugEx.LogWarning("Game Over!");
    }


    void Hit()
    {
        Managers.Audio.PlaySound("HealthDown");
        CurrentLives--;
        if(CurrentLives <= 0 )
        {
            KillPlayer();
        }
    }

    void EAT(Food food)
    {
        Managers.Audio.PlaySound("Pickup");
        switch (food.GetFoodType())
        {
            case FoodType.Baechu:
                CurrentLives = Mathf.Min(3, CurrentLives + 1);
                break;
            case FoodType.Garlic:
                Managers.Game.CurrentScore *= 2;
                break;
            case FoodType.Gochu:
                CurrentLives = Mathf.Min(3, CurrentLives + 1);
                break;

        }
        
    }

    IEnumerator GoldenTimeCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(10f);
        isInvincible = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag=="Enemy")
        {
            if(!isInvincible)
            {
                Managers.Game.Despawn(collision.gameObject);
                Hit();
            }
        }
        else if(collision.gameObject.tag=="Golden")
        {
            Managers.Game.Despawn(collision.gameObject);
            StartCoroutine(GoldenTimeCoroutine());
        }
        else if(collision.gameObject.tag=="food")
        {
            Managers.Game.Despawn(collision.gameObject);
            EAT(collision.gameObject.GetComponent<Food>());
        }
    }

    private void OnEnable()
    {
        // performed 이벤트에 점프 메서드 등록
        playerInputActions.PlayerAction.Jump.performed += OnJumpPerformed;
        playerInputActions.PlayerAction.Jump.canceled += OnJumpCanceled;
        playerInputActions.Enable();
    }

   

    private void OnDisable()
    {
        playerInputActions.Disable();
    }
}
