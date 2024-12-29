using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    // State interface and implementations
    public interface IPlayerState
    {
        void Enter(Player player);
        void Execute();
        void Exit();
    }

    private class IdleState : IPlayerState
    {
        private Player _player;

        public void Enter(Player player)
        {
            _player = player;
            _player.animator.SetInteger("state", 0);
        }

        public void Execute()
        {
            // Stay idle until specific input is given
            if (!_player.isGrounded) _player.ChangeState(_player._fallState);
        }

        public void Exit() { }
    }

    private class JumpState : IPlayerState
    {
        private Player _player;

        public void Enter(Player player)
        {
            _player = player;
            _player.animator.SetInteger("state", 1);
            _player.rb.AddForce(Vector2.up * _player.jumpForce, ForceMode2D.Impulse);
            Managers.Audio.PlaySound("Jump");
            _player.isGrounded = false;
        }

        public void Execute()
        {
            if (_player.rb.linearVelocityY <= 0) _player.ChangeState(_player._fallState);
        }

        public void Exit() { }
    }

    private class FallState : IPlayerState
    {
        private Player _player;

        public void Enter(Player player)
        {
            _player = player;
            _player.animator.SetInteger("state", 2);
        }

        public void Execute()
        {
            // Transition to Idle state when grounded
            if (_player.isGrounded) _player.ChangeState(_player._idleState);
        }

        public void Exit() { }
    }

    private class HitState : IPlayerState
    {
        private Player _player;

        public void Enter(Player player)
        {
            _player = player;
            Managers.Audio.PlaySound("HealthDown");
            _player.CurrentLives--;

            if (_player.CurrentLives <= 0) _player.ChangeState(_player._deathState);
        }

        public void Execute() { }

        public void Exit() { }
    }

    private class DeathState : IPlayerState
    {
        private Player _player;

        public void Enter(Player player)
        {
            _player = player;
            player.capsuleCollider.enabled = false;
            player.animator.enabled = false;
            player.rb.AddForce(Vector2.up * player.jumpForce, ForceMode2D.Impulse);
            Managers.Audio.PlaySound("GameOver");
            _player.StartCoroutine(_player.GameOverRoutine());
        }

        public void Execute() { }

        public void Exit() { }
    }

    // Fields
    [SerializeField] private float jumpForce = 10;
    [SerializeField] private int currentLives = 3;

    private Rigidbody2D rb;
    private Animator animator;
    private CapsuleCollider2D capsuleCollider;
    private SpriteRenderer spriteRenderer;

    private bool isGrounded = true;
    private bool isInvincible = false;

    private IPlayerState _currentState;
    private IdleState _idleState;
    private JumpState _jumpState;
    private FallState _fallState;
    private HitState _hitState;
    private DeathState _deathState;

    public event Action<int> OnHealthChanged;
    private PlayerInputActions playerInputActions;

    // Properties
    public int CurrentLives
    {
        get => currentLives;
        set
        {
            if (currentLives != value)
            {
                currentLives = Mathf.Clamp(value, 0, 3);
                OnHealthChanged?.Invoke(currentLives);
            }
        }
    }

    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Input System 설정
        playerInputActions = new PlayerInputActions();
        playerInputActions.PlayerAction.Jump.performed += JumpAction;

        // Initialize states
        _idleState = new IdleState();
        _jumpState = new JumpState();
        _fallState = new FallState();
        _hitState = new HitState();
        _deathState = new DeathState();

        ChangeState(_idleState);
    }

    private void FixedUpdate()
    {
        _currentState?.Execute();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnEnable()
    {
        playerInputActions.PlayerAction.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.PlayerAction.Disable();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && !isInvincible)
        {
            Managers.Game.Despawn(collision.gameObject);
            ChangeState(_hitState);
        }
        else if (collision.CompareTag("Golden"))
        {
            Managers.Game.Despawn(collision.gameObject);
            StartCoroutine(GoldenTimeRoutine());
        }
        else if (collision.CompareTag("food"))
        {
            Managers.Game.Despawn(collision.gameObject);
            EAT(collision.GetComponent<Food>());
        }
    }

    private void ChangeState(IPlayerState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter(this);
    }

    private void JumpAction(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            ChangeState(_jumpState);
        }
    }

    private IEnumerator GoldenTimeRoutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(10f);
        isInvincible = false;
    }

    private IEnumerator GameOverRoutine()
    {
        yield return new WaitForSeconds(3f);
        Managers.Game.SaveHighScore();
        Managers.UI.ShowPopupUI<UI_GameOver>();
    }

    private void EAT(Food food)
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
}
