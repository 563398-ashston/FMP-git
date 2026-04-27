using UnityEngine;

public class SpikeDamage : MonoBehaviour
{
    public int damage = 10;
    private HealthScript playerHealth;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get components
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (playerHealth == null)
            {
                playerHealth = collision.gameObject.GetComponent<HealthScript>();
            }

            // Deal damage
            playerHealth.TakeDamage(damage);

            // Teleport player to their current checkpoint
            collision.transform.position = player.resetPoint.position;

            //playerHealth.ResetHealth();
        }
    }
}