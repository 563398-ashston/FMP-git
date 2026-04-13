using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
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
    public float dashSpeed = 20f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 0.1f;
    [SerializeField] private TrailRenderer tr;

    bool isDashing;
    bool canDash = true;
    TrailRenderer trailRenderer;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction dashAction;

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

        if (isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (!isFacingRight && horizontal < 0f)
        {
            Flip();
        }

        //walking animation
        anim.SetFloat("Speed", Mathf.Abs(horizontal));

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

    private bool IsGrounded()
    {
        bool grounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (grounded)
        {
            
        }
        return grounded;
    }

    public void Move()
        {
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
        if (dashAction.IsPressed() && IsGrounded())
        {
            rb.linearVelocity = new Vector2 (rb.linearVelocity.x, dashSpeed);

            print("dashed");
        }
    }
}
