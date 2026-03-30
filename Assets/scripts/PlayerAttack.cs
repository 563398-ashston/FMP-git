using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    public float attackCooldown = 0.5f;

    private bool canAttack = true;

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack)
        {
            Attack();
        }
    }

    void Attack()
    {
        canAttack = false;
        animator.SetTrigger("Attack");

        Invoke(nameof(ResetAttack), attackCooldown);
    }
    void ResetAttack()
    {
        canAttack = true;
    }
}