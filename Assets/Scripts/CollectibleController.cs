using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player collided with the collectible
        if (other.CompareTag("Player"))
        {
            PlayerController.Instance.Health += 10;  // update the score
            Destroy(gameObject);  // destroy the collectible
            print("Health: " + PlayerController.Instance.Health);
            PlayerController.Instance.UpdateHealthUI();
        }
    }
}
