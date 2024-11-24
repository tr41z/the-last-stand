using UnityEngine;

public class PlayerDefend : MonoBehaviour
{
    private Animator anim;
    private bool isDefending;
    public static PlayerDefend Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Check if the player is in DefendZone before allowing defend
        if (PlayerController.Instance.IsInDefendZone && Input.GetKey(KeyCode.Mouse1) && PlayerController.Instance.Grounded && !PlayerController.Instance.IsJumping)
        {
            Defend();
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
            StopDefending();
    }

    private void Defend()
    {
        anim.Play("Defend");
        isDefending = true;
    }

    private void StopDefending()
    {
        anim.SetTrigger("releaseDefend");
        isDefending = false;
    }

    public bool IsDefending() => isDefending;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            PlayerController.Instance.Grounded = true;
            PlayerController.Instance.IsJumping = false;
            isDefending = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            PlayerController.Instance.Grounded = false;
            PlayerController.Instance.IsJumping = true;
        }
    }
}
