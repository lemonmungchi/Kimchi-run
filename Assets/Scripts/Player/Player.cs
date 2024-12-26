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

    [SerializeField] private int currentLives = 3; // ���� ��� �� ����

    private bool isInvincible =false;

    // �ǰ� ����Ʈ �ð�ȭ�� ���� �ʵ��
    private SpriteRenderer spriteRenderer;            // ��������Ʈ ������ ����Ʈ

    // �� Input System���� �ڵ� �����Ǵ� Ŭ����(���̹� ��Ģ�� ���� ���� ����)
    private PlayerInputActions playerInputActions;
    // ü�� ���� �̺�Ʈ
    public event System.Action<float> OnHealthChanged;

    private void Awake()
    {
        // InputActions �ν��Ͻ� ����
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
            if (currentLives != value) // ����� ����� ���� ����
            {
                currentLives = Mathf.Clamp(value, 0, 3); // �� ���� ����
                OnHealthChanged?.Invoke(currentLives); // �̺�Ʈ ȣ��
            }
        }
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
        Managers.Audio.PlaySound("Jump");
    }

    // Ground�� �浹���� �� ������ �ν�
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
    /// �������� ���� �� �ð�ȭ
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlashRed()
    {
        isInvincible = true; // ���� ���� ����

        for (int i = 0; i < 3; i++) // �� ���ʿ� ���� ������
        {
            // ��� ��������Ʈ ������ ���������� ����
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f); // 0.1�� ���

            // ��� ��������Ʈ ������ ���� �������� ����
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f); // 0.1�� ���
        }

        isInvincible = false; // ���� ���� ����
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
        yield return new WaitForSeconds(3f); // 2�� ���
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
