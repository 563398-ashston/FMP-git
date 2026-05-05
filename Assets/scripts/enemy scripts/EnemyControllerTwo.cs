using Unity.VisualScripting;
using UnityEngine;

public class EnemyControllerTwo : MonoBehaviour 
{ 
    [SerializeField] private float 
    speed = 1.5f; private GameObject player;
    public States state;
    Animator anim;
    Rigidbody2D rb;


    //health
    [SerializeField] private int maxHealth = 100;
    public int currentHealth;

    public enum States
    {
        move,
        takeDamage,
        dying
    }

    void Start() 
    { 
        player = GameObject.FindGameObjectWithTag("Player");
        currentHealth = maxHealth;
    }

    private void Update()
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
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime); 
    }

    public void TakeDamage(int damage)
    {
        print("enemy2: i am damaged");
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            state = States.dying;
        }
        else
        {
            state = States.takeDamage;
            //anim.Play("enemy_2_hurt");

            // return to move after short delay
            Invoke(nameof(ReturnToMove), 0.25f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Spike")
        {
            currentHealth = 0;
            state = States.dying;
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
}