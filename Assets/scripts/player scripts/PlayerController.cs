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
    private HealthScript playerHealth;
    public Transform resetPoint;
    private bool isFacingRight = true;
    private float horizontal;

    [Header("player values")]
    [SerializeField] float speed = 8f;
    [SerializeField] float jumpingPower = 16;
    public int jumpsLeft;
    [SerializeField] float maxVertSpeed;

    [Header("dashing values")]
    public float dashForce = 30f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    public bool isDashing;
    private float dashTimer;
    private float cooldownTimer;
    private bool hasDashedInAir;

    [Header("attack values")]
    public bool isAttacking;
    public int attackDamage = 50;
    public float attackRange = 0.5f;
    public float attackrate = 2f;
    private float attackTimer;
    [SerializeField] float attackDuration = 0.2f;
    private float nextAttackTime = 0f;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    private int maxJumps = 2;
    private float originalGravity;

    //input actions
    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;
    InputAction attackAction;

    public enum PlayerAnimation
    {
        Idle,
        Walking,
        Jumping,
        Falling,
        Dashing,
        DoubleJumping,
        Attacking
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
        attackAction = InputSystem.actions.FindAction("Attack");

        jumpsLeft = maxJumps;
        originalGravity = rb.gravityScale;
    }

    private void OnEnable() => dashAction.Enable();
    private void OnDisable() => dashAction.Disable();

    private void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude > maxVertSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxVertSpeed;
        }
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Spike")
        {
            collision.gameObject.GetComponent<PlayerController>().resetPoint = resetPoint;
            //collision.gameObject.GetComponent<HealthScript>().ResetHealth();
            //print("reset point is " + resetPoint);
        }
    }
    


    private void Update()
    {
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;

            if (attackTimer <= 0f)
            {
                isAttacking = false;
            }
        }

        Jump();
        Move();
        Dash();

        if (attackAction.WasPressedThisFrame())
        {
            Attack();
        }

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
        else if (isAttacking == true)
        {
            currentAnim = PlayerAnimation.Attacking;
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
            case PlayerAnimation.Attacking: return "attack_anim";
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

    public void Attack()
    {
        isAttacking = true;
        attackTimer = attackDuration;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("we hit " + enemy.name);

            DoEnemyDamage(enemy.gameObject);
           
        }
    }


    void DoEnemyDamage(GameObject enemy)
    {
        if (enemy.tag == "enemy1")
        {
            enemy.GetComponent<EnemyControllerOne>().TakeDamage(attackDamage);
        }
        if (enemy.tag == "enemy2")
        {
            enemy.GetComponent<EnemyControllerTwo>().TakeDamage(attackDamage);
        }


    }


    private void OnDrawGizmos()
    {
        if(attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
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

       //if (isAttacking)
    }
}
