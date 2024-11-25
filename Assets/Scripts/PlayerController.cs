using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }  // singleton instance
    public static event Action OnPlayerDeath;  // event for player death
    public static event Action OnPlayerRespawn;  // event for player respawn
    private int health;
    public int Health { get => health; set => health = Mathf.Max(0, value); }  // health property with clamping
    public bool Grounded { get; set; }
    public bool IsJumping { get; set; }
    private Animator anim;
    private Rigidbody2D rb;
    public bool IsInDefendZone { get; private set; }
    private bool isNearRune = false;  // flag to track if near a rune
    private Collider2D currentRune;  // reference to the current rune collider
    public Text healthText;
    private bool isRespawning = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);  // enforce singleton pattern

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Grounded = true;
        IsJumping = false;
        health = 100;
        UpdateHealthUI();
    }

    private void Update()
    {
        if (health <= 0)
            Die();

        // Check if the player presses E while near a rune
        if (isNearRune && Input.GetKeyDown(KeyCode.E))
        {
            PlayerAttack.Instance.DamageAmount += 30;
            Destroy(currentRune.gameObject);  // destroy the rune
            print("Damage Amount: " + PlayerAttack.Instance.DamageAmount);

            isNearRune = false;  // reset the flag
            currentRune = null;  // clear the rune reference
        }
    }

    public void TakeDamage(int amount, Animator anim)
    {
        anim.Play("Damage");
        Health -= amount;
        UpdateHealthUI();
    }

    public void Die()
    {
        if (health > 0 || isRespawning) return;  // ensure Die logic runs only once per death
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        isRespawning = true;  // mark as respawning
        rb.isKinematic = true;

        anim.SetBool("run", false);
        anim.Play("Death");
        OnPlayerDeath?.Invoke();  // trigger the death event for external listeners

        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        float deathAnimationLength = anim.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(deathAnimationLength + 3f);  // wait for animation + 3 seconds

        ResetPlayer();
        isRespawning = false;  // reset the flag
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DefendZone")) HandleDefendZoneEnter();

        if (other.CompareTag("Rune"))
        {
            isNearRune = true;  // set flag when near a rune
            currentRune = other;  // store reference to the rune
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("DefendZone"))
        {
            print("EXITED DEFEND ZONE");
            IsInDefendZone = false; // set the flag to false when leaving DefendZone
        }

        if (other.CompareTag("Rune"))
        {
            isNearRune = false;  // clear flag when leaving the rune area
            currentRune = null;  // clear rune reference
        }
    }

    private void HandleDefendZoneEnter()
    {
        print("ENTERED DEFEND ZONE");
        IsInDefendZone = true;
    }

    // Update the health display on the UI
    public void UpdateHealthUI()
    {
        healthText.text = health.ToString();
    }

    public void ResetPlayer()
    {
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        health = 100;
        UpdateHealthUI();
        rb.isKinematic = false;
        anim.SetBool("run", false);
        anim.Play("Idle");
        transform.position = new Vector3(-19.0f, 0.0f);  // reset position
        OnPlayerRespawn?.Invoke();  // trigger respawn event
    }
}
