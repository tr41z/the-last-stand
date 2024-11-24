using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }  // singleton instance
    public static event Action OnPlayerDeath;  // event for player death
    private int health;
    public int Health { get => health; set => health = Mathf.Max(0, value); }  // health property with clamping
    public bool Grounded { get; set; }
    public bool IsJumping { get; set; }
    private Animator anim;
    private Rigidbody2D rb;
    public bool IsInDefendZone { get; private set; }
    private bool isNearRune = false;  // flag to track if near a rune
    private Collider2D currentRune;  // reference to the current rune collider

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
        Debug.Log("Player Health: " + Health);
    }

    public void Die()
    {
        if (health > 0) return;

        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        transform.localScale = Vector3.one;

        anim.SetBool("run", false);
        anim.Play("Death");
        OnPlayerDeath?.Invoke();  // trigger the event
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
}
