using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    Animator anim;

    private bool isFacingRight = true;
    private float horizontal;

    [Header("player values")]
    [SerializeField] float speed = 8f;
    [SerializeField] float jumpingPower = 16;

    [Header("dashing values")]
    public float dashForce = 30f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;

    public bool isDashing;
    private float dashTimer;
    private float cooldownTimer;

    [SerializeField] private TrailRenderer tr;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;

    public enum PlayerAnimation
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Dashing
    }

    private void Awake()
    {
        rb.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash");
    }

    private void OnEnable() => dashAction.Enable();
    private void OnDisable() => dashAction.Disable();

    private void Update()
    {
        Jump();
        Move();
        Dash();
        FlipCheck();

        UpdateAnimation();
    }

    
    void UpdateAnimation()
    {
        PlayerAnimation currentAnim;

        if (isDashing)
        {
            currentAnim = PlayerAnimation.Dashing;
        }
        else if (!IsGrounded() && rb.linearVelocity.y > 0.1f)
        {
            currentAnim = PlayerAnimation.Jumping;
        }
        else if (!IsGrounded() && rb.linearVelocity.y < -0.1f)
        {
            currentAnim = PlayerAnimation.Falling;
        }
        else if (Mathf.Abs(horizontal) > 0.1f)
        {
            currentAnim = PlayerAnimation.Walking;
        }
        else
        {
            currentAnim = PlayerAnimation.Idle;
        }

      
        anim.Play(GetAnimationName(currentAnim));
    }

   
    string GetAnimationName(PlayerAnimation animType)
    {
        switch (animType)
        {
            case PlayerAnimation.Idle: return "idle_anim";
            case PlayerAnimation.Walking: return "walking_anim";
            case PlayerAnimation.Jumping: return "jumping_anim";
            case PlayerAnimation.Falling: return "falling_anim";
            case PlayerAnimation.Dashing: return "dash_anim";
            default: return "idle anim";
        }
    }


    private void FlipCheck()
    {
        if (isFacingRight && horizontal > 0f || !isFacingRight && horizontal < 0f)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void Move()
    {
        if (isDashing) return;

        horizontal = moveAction.ReadValue<Vector2>().x;
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }

    public void Jump()
    {
        if (jumpAction.IsPressed() && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }

        if (!jumpAction.IsPressed() && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    public void Dash()
    {
        cooldownTimer -= Time.deltaTime;

        if (dashAction.WasPressedThisFrame() && cooldownTimer <= 0f)
        {
            StartDash();
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                isDashing = false;
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                tr.emitting = false;
            }
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashTime;
        cooldownTimer = dashCooldown;

        float direction = Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = new Vector2(-direction * dashForce, rb.linearVelocity.y);
        tr.emitting = true;
    }
}
