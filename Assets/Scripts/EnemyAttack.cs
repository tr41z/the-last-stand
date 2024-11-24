using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 1f; // time between attacks
    [SerializeField] private int damageAmount = 10; // damage dealt to the player
    [SerializeField] private float delay;
    private float attackTimer = 0f; // timer to track cooldown
    private bool isPlayerInRange = false; // track if player is in range
    private Animator anim; // animator for attack animation
    private EnemyController enemyController; // reference to EnemyController

    void Awake()
    {
        anim = GetComponent<Animator>();
        enemyController = GetComponent<EnemyController>();
    }

    void Update()
    {
        // Prevent any behavior updates if the enemy is dead
        if (enemyController != null && enemyController.IsDead)
        {
            return; // stop attacking if the enemy is dead
        }

        // Prevent attacking if the player is dead
        if (PlayerController.Instance == null || PlayerController.Instance.Health <= 0)
        {
            return;
        }

        // Update the attack timer
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // Attempt to attack the player if in range and cooldown is over
        if (isPlayerInRange && attackTimer <= 0)
        {
            AttackPlayer();
            attackTimer = attackCooldown; // reset the cooldown
        }
    }

    private void AttackPlayer()
    {
        // Stop movement during attack
        if (enemyController != null)
        {
            enemyController.IsAttacking = true;
        }

        // Play attack animation
        if (anim != null)
        {
            anim.Play("Attack");
        }

        // Apply damage after a delay, synchronized with animation
        StartCoroutine(ApplyDamageAtAnimationEnd());
    }

    private IEnumerator ApplyDamageAtAnimationEnd()
    {
        // Wait for animation to finish
        yield return new WaitForSeconds(delay);

        // Check if the player is still in range before applying damage
        if (isPlayerInRange)
        {
            var player = PlayerController.Instance; // access the player via the singleton instance
            if (player != null)
            {
                player.TakeDamage(damageAmount, player.GetComponent<Animator>());
            }
        }

        // Allow movement after attack
        if (enemyController != null)
        {
            enemyController.IsAttacking = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true; // player is now within attack range
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false; // player has left attack range
        }
    }
}
