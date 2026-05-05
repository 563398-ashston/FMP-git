using Unity.VisualScripting;
using UnityEngine;

public class EnemyControllerOne : MonoBehaviour
{
    bool isGrounded;
    public LayerMask groundLayerMask;
    public States state;
    Animator anim;
    Rigidbody2D rb;
    [SerializeField] public float xvel;

    //health
    [SerializeField] private int maxHealth = 100;
    public int currentHealth;

    public enum States
    {
       move,
       takeDamage,
       dying
    }

    public void HitPlayer(Transform playerTransform)
    {
        //FindObjectOfType<HealthScript>.TakeDamage;
    }

    void Start()
    {
        groundLayerMask = LayerMask.GetMask("Ground");
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        xvel = 5;

        //states
        state = States.move;
    }

    void Update()
    {
        switch (state)
        {
            case States.move:
                Move();
                break;

            case States.takeDamage:
                break;

            case States.dying:
                Die();
                break;
        }
    }

    private void Move()
    { 
        if (xvel < 0)
        {
            if (ExtendedRayCollisionCheck(-1, 0) == false)
            {
                xvel = 5;
                gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        if (xvel > 0)
        {
            if (ExtendedRayCollisionCheck(1, 0) == false)
            {
                xvel = -5;
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
            }
        }

        rb.linearVelocity = new Vector2(xvel, 0);

        anim.Play("enemy_1_walking");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            state = States.dying;
        }
        else
        {
            state = States.takeDamage;
            anim.Play("enemy_1_hurt");

            //return to move after short delay
            Invoke(nameof(ReturnToMove), 0.25f);
        }
    }

    void ReturnToMove()
    {
        if (state != States.dying)
            state = States.move;
    }

    public void Die()
    {
        anim.Play("enemy_1_dying");

        GetComponent<Collider2D>().enabled = false;
        rb.linearVelocity = Vector2.zero;

        // destroy after animation
        Destroy(gameObject, 0.43f);
    }

    public bool ExtendedRayCollisionCheck(float xoffs, float yoffs)
    {
        float rayLength = 0.5f; // length of raycast 
        bool hitSomething = false;

        //convert x and y offset into a Vector 3
        Vector3 offset = new Vector3(xoffs, yoffs, 4);

        //cast a ray downwards 
        RaycastHit2D hit;


        hit = Physics2D.Raycast(transform.position + offset, -Vector2.up, rayLength, groundLayerMask);

        Color hitColor = Color.white;

        if (hit.collider != null)
        {
            //print("player has collided with ground layer");
            hitColor = Color.green;
            hitSomething = true;
        }

        Debug.DrawRay(transform.position + offset, Vector2.down * rayLength, hitColor);
        return hitSomething;
    }
}