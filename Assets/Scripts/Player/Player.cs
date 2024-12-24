using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    [SerializeField] private float jumpForce;
    private Rigidbody2D rb;
    private bool isGrounded = true;
    private Animator animator;

    // �� Input System���� �ڵ� �����Ǵ� Ŭ����(���̹� ��Ģ�� ���� ���� ����)
    private PlayerInputActions playerInputActions;

    private void Awake()
    {
        // InputActions �ν��Ͻ� ����
        rb= GetComponent<Rigidbody2D>();
        playerInputActions = new PlayerInputActions();
        animator = GetComponent<Animator>();
    }

    // ���� �ݹ��Լ�
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

    // ���� ���� ����
    private void Jump()
    {
        
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    // Ground�� �浹���� �� ������ �ν�
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
        // performed �̺�Ʈ�� ���� �޼��� ���
        playerInputActions.PlayerAction.Jump.performed += OnJumpPerformed;
        playerInputActions.PlayerAction.Jump.canceled += OnJumpCanceled;
        playerInputActions.Enable();
    }

   

    private void OnDisable()
    {
        playerInputActions.Disable();
    }
}
