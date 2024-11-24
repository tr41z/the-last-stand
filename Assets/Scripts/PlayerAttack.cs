using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public static PlayerAttack Instance { get; private set; }  // singleton instance
    float timer = 0;
    [SerializeField] float timerSet; // the time between combo hits (cooldown)
    [SerializeField] float attackCooldown = 0.25f; // cooldown time between attacks
    float attackTimer = 0; // timer to track cooldown between attacks
    int comboHits;
    Animator anim;
    private bool grounded;
    private bool isJumping;
    [SerializeField] public int DamageAmount = 10;
    private bool isInCollisionZone = false; // to track if the player is in collision with the enemy
    private Collider2D enemyCollider; 

    void Awake()
    {
        if (Instance == null)
            Instance = this; // Assign the singleton instance
        else
            Destroy(gameObject); // Ensure there is only one instance

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Update attack cooldown timer
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // Attack only if Mouse0 is pressed, the player is grounded, not jumping, and cooldown is over
        if (Input.GetKeyDown(KeyCode.Mouse0) && grounded && !isJumping && attackTimer <= 0)
        {
            switch (comboHits)
            {
                case 0:
                    FirstHit();
                    break;
                case 1:
                    SecondHit();
                    break;
                case 2:
                    FinalHit();
                    break;
            }

            // Reset attack cooldown
            attackTimer = attackCooldown;
        }

        if (timer > 0)
            timer -= Time.deltaTime;
        else
            comboHits = 0;
    }

    void FirstHit()
    {
        anim.Play("Attack");
        comboHits++;
        timer = timerSet;
        AttemptDamage();
    }

    void SecondHit()
    {
        anim.Play("Attack1");
        comboHits++;
        timer = timerSet;
        AttemptDamage();
    }

    void FinalHit()
    {
        anim.Play("Attack2");
        comboHits = 0;
        timer = 0;
        AttemptDamage();
    }

    // Method to attempt to damage the enemy only if in collision zone
    void AttemptDamage()
    {
        if (isInCollisionZone && enemyCollider != null)
        {
            var enemy = enemyCollider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(DamageAmount, transform.position);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
            isJumping = false; // stop jumping when landing
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
            isJumping = true; // mark as jumping when leaving the ground
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            isInCollisionZone = true; // player is in the enemy's collision zone
            enemyCollider = other; // store a reference to the enemy's collider
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            isInCollisionZone = false; // player is no longer in the enemy's collision zone
            enemyCollider = null; // clear the reference
        }
    }
}
