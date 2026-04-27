using UnityEngine;

public class respawnScript : MonoBehaviour
{
    public Transform resetPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().resetPoint = resetPoint;
            //collision.gameObject.GetComponent<HealthScript>().ResetHealth();

            print("reset point is " + resetPoint);
        }
    }
}