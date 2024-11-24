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
    public bool IsInRoom2 { get; private set; }

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
        if (other.CompareTag("Room2")) 
        {
            print("ENTERED LEVEL 2");
            IsInRoom2 = true; // set the flag to true when entering Room2
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Room2"))
        {
            print("EXITED LEVEL 2");
            IsInRoom2 = false; // set the flag to false when leaving Room2
        }
    }
}
