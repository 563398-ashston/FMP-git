using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
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
    [SerializeField] float maxVertSpeed;

    [Header("dashing values")]
    public float dashForce = 30f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    public bool isDashing;
    private float dashTimer;
    private float cooldownTimer;
    private bool hasDashedInAir;

    private int maxJumps = 2;
    public int jumpsLeft;
    private float originalGravity;

    

    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;

    public enum PlayerAnimation
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Dashing,
        DoubleJumping
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
        jumpsLeft = maxJumps;
        originalGravity = rb.gravityScale;
    }

    private void OnEnable() => dashAction.Enable();
    private void OnDisable() => dashAction.Disable();

    private void FixedUpdate()
    {
        if(rb.linearVelocity.magnitude > maxVertSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVertSpeed;
        }
    }

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
        else if (!IsGrounded() && rb.linearVelocity.y > 0.1f && jumpsLeft == 0)
        {
            currentAnim = PlayerAnimation.DoubleJumping;
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
            case PlayerAnimation.DoubleJumping: return "double_jump_anim";
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
        return Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);
    }

    public void Move()
    {
        if (isDashing) return;

        horizontal = moveAction.ReadValue<Vector2>().x;
        rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
    }

    public void Jump()
    {

        if (isDashing) return;

        //print("vy=" + rb.linearVelocity.y + "  grounded=" + IsGrounded() );
        if (IsGrounded() && jumpAction.WasPressedThisFrame() )
        {
            jumpsLeft = maxJumps;
            //print("reset jumps to " + maxJumps);
        }

        if (jumpAction.WasPressedThisFrame() && jumpsLeft > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            jumpsLeft--;
        }

        if (!jumpAction.IsPressed() && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }



    public void Dash()
    {
        cooldownTimer -= Time.deltaTime;

        bool grounded = IsGrounded();

        if (grounded)
        {
            hasDashedInAir = false;
        }

        if (dashAction.WasPressedThisFrame())
        {
            if (grounded)
            {
                if (cooldownTimer <= 0f)
                {
                    StartDash();
                }
            }
            else
            {
                if (!hasDashedInAir)
                {
                    StartDash();
                    hasDashedInAir = true;
                }
            }
        }
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                isDashing = false;
                rb.gravityScale = originalGravity;
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                
            }
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashTime;
        cooldownTimer = dashCooldown;

        float direction = Mathf.Sign(transform.localScale.x);

        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(-direction * dashForce, 0f);

       
    }
}
