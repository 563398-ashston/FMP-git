using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public GameObject attackPoint;
    public float radius;
    Animator anim;
    public LayerMask enemies;
    //InputAction attackAction;
    
    private void Start()
    {
        //attackAction = InputSystem.actions.FindAction("Attack");
    }

    public void attack()
    {
        anim.Play("attack_anim");






        
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);

        foreach (Collider2D enemyGameObject in enemy)
        {
            Debug.Log("hit enemy");
        }
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
    }
    
}
