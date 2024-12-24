using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    [SerializeField] private float jumpForce;
    private Rigidbody2D rb;
    private bool isGrounded = true;
    private Animator animator;

    // 새 Input System에서 자동 생성되는 클래스(네이밍 규칙에 따라 변경 가능)
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        // InputActions 인스턴스 생성
        rb= GetComponent<Rigidbody2D>();
        playerInputActions = new PlayerInputActions();
        animator = GetComponent<Animator>();
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
    }

    // Ground와 충돌했을 때 땅으로 인식
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Equals("Ground"))
        {
            if(!isGrounded)
            {
                animator.SetInteger("state", 2);
            }
            isGrounded = true;
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
