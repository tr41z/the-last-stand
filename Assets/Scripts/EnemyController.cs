using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private int health;
    private Rigidbody2D rb;
    private Animator anim;
    private float knockbackForce = 4f;
    private Transform player;
    public float followSpeed = 2f;
    private bool isKnockedBack = false; // track if knockback is active
    private bool isDead = false; // track if the enemy is dead
    public bool IsAttacking { get; set; } = false; // track if the enemy is attacking
    public bool IsDead => isDead;
    private float detectionRadius = 10f;
    [SerializeField] private float minimumDistance = 1.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();  
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player").transform; // assign the player object by tag
        PlayerController.OnPlayerDeath += HandlePlayerDeath;  // subscribe to player death event
        PlayerController.OnPlayerRespawn += ResetEnemy;  // subscribe to player respawn event
    }

    void OnDestroy()
    {
        PlayerController.OnPlayerDeath -= HandlePlayerDeath;  // unsubscribe from player death event
        PlayerController.OnPlayerRespawn -= ResetEnemy;  // unsubscribe from player respawn event
    }

    void Update()
    {
        // Prevent any behavior updates if the enemy is dead or player is dead
        if (isDead || PlayerController.Instance == null || PlayerController.Instance.Health <= 0) 
        {
            anim.SetBool("run", false); // ensure running stops after death
            return; 
        }

        if (!isKnockedBack)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRadius && !IsAttacking) // only follow when within range and not attacking
            {
                FollowPlayerOnXAxis();
            }
            else
            {
                anim.SetBool("run", false); // ensure animation stops while attacking
            }
        }
    }

    private void FollowPlayerOnXAxis()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Only move if the enemy is farther than the minimum distance
        if (distanceToPlayer > minimumDistance)
        {
            anim.SetBool("run", true);
            float step = followSpeed * Time.deltaTime;
            Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, step);

            // Flip the sprite based on the direction
            if (transform.position.x < player.position.x && transform.localScale.x < 0)
            {
                // Enemy is to the left of the player, face right
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (transform.position.x > player.position.x && transform.localScale.x > 0)
            {
                // Enemy is to the right of the player, face left
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            anim.SetBool("run", false); // stop running animation when too close
        }
    }

    public void TakeDamage(int damage, Vector2 playerPosition)
    {
        // Ignore damage if already dead or player is dead
        if (isDead || PlayerController.Instance == null || PlayerController.Instance.Health <= 0) return;

        health -= damage;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            anim.Play("Damage");
            StartCoroutine(DelayedKnockback(playerPosition, 0.05f));
        }
    }

    private void Die()
    {
        isDead = true; // set the dead flag to stop movement
        Destroy(GetComponent<BoxCollider2D>()); // disable collisions
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;

        anim.Play("Death"); // play death animation

        // Delay destruction based on animation length
        float deathAnimationLength = anim.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, deathAnimationLength + 3f);
    }

    public void Knockback(Vector2 playerPosition)
    {
        if (isDead) return; // no knockback if already dead

        Vector2 difference = (transform.position - (Vector3)playerPosition).normalized;
        Vector2 force = difference * knockbackForce;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private IEnumerator DelayedKnockback(Vector2 playerPosition, float delay)
    {
        if (isDead) yield break; // stop if dead

        isKnockedBack = true; // disable following
        yield return new WaitForSeconds(delay);
        Knockback(playerPosition);
        yield return new WaitForSeconds(0.2f); 
        isKnockedBack = false; // re-enable following
    }

    private void HandlePlayerDeath()
    {
        anim.SetBool("run", false);  // stop running animation
        isKnockedBack = true;  // prevent further movement
        IsAttacking = false;  // prevent attacking
    }

    private void ResetEnemy()
    {
        isDead = false;
        isKnockedBack = false;
        IsAttacking = false;
        health = 100;  // reset health or set to initial value
        rb.isKinematic = false;
        anim.SetBool("run", false);
        anim.Play("Idle");
    }
}
