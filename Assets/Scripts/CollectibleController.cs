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
            PlayerController.Instance.UpdateHealthUI();
        }
    }
}
