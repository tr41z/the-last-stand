using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    private Rigidbody2D body;
    private Animator anim;
    private PlayerDefend playerDefend;
    public AudioSource jumpSound;
    public AudioSource runSound;
    private bool isRunning; 

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerDefend = GetComponent<PlayerDefend>();
    }

    private void Update() 
    {
        if (playerDefend.IsDefending())  // prevent movement when defending
            return;

        float horizontalInput = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

        if (horizontalInput > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);

        if (Input.GetKey(KeyCode.Space) && PlayerController.Instance.Grounded)
            Jump();

        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", PlayerController.Instance.Grounded);

        HandleRunningSound(horizontalInput); // handle running sound logic
    }

    private void HandleRunningSound(float horizontalInput)
    {
        // Check if the player is moving horizontally (running)
        if (horizontalInput != 0 && !isRunning)
        {
            runSound.Play(); // play the running sound if not already playing
            isRunning = true;
        }
        else if (horizontalInput == 0 && isRunning || PlayerController.Instance.IsJumping)
        {
            runSound.Stop(); // stop the running sound if not moving
            isRunning = false;
        }
    }

    private void Jump() 
    {
        body.velocity = new Vector2(body.velocity.x, jumpForce);
        jumpSound.Play();
        anim.SetTrigger("jump");
        PlayerController.Instance.Grounded = false;
        PlayerController.Instance.IsJumping = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
            PlayerController.Instance.Grounded = true;
    }
}
