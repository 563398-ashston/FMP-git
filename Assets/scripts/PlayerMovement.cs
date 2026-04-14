using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.XR;
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

    private void Awake()
    {
        rb.GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        dashAction.Enable();
    }
    private void OnDisable()
    {
        dashAction.Disable();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash");
    }

    private void Update()
    {
        Jump();
        Move();
        Dash();

        //flip code
        if (isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (!isFacingRight && horizontal < 0f)
        {
            Flip();
        }

        //walking animation
        if (!isDashing)
        {
            anim.SetFloat("Speed", Mathf.Abs(horizontal));
        }

        //dash animation
        if (isDashing)
        {
            anim.SetFloat("Speed", 0f);
        }

        //jumping animation
        float verticalVelocity = rb.linearVelocity.y;

        if (IsGrounded())
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
        }
        else
        {
            if (verticalVelocity > 0.1f)
            {
                anim.SetBool("isJumping", true);
                anim.SetBool("isFalling", false);
            }
            else if (verticalVelocity < -0.1f)
            {
                anim.SetBool("isJumping", false);
                anim.SetBool("isFalling", true);
            }
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    //grounded check for the jump
    private bool IsGrounded()
    {
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (grounded)
        {
            
        }
        return grounded;
    }
    
    //Player movement 
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
            rb.linearVelocity = new Vector2(rb.linearVelocity.x,jumpingPower);
            
            print("jumped");
        }
        if (jumpAction.IsPressed()==false && rb.linearVelocity.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            
            print("falling");
        }
    }

    public void Dash()
    {
        //Dash code
        cooldownTimer -= Time.deltaTime;

        if (dashAction.WasPressedThisFrame() && cooldownTimer <= 0f)
        {
            StartDash();
            Debug.Log("dash pressed");
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0f)
            {
                isDashing = false;

                anim.SetBool("isDashing", false);

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

        isDashing = true;
        anim.SetBool("isDashing", true);

        float direction = Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = new Vector2(-direction * dashForce, rb.linearVelocity.y);
        tr.emitting = true;

        
    }
}
