using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    //Audio source used to play sound effects.
    public AudioSource soundEffectSource;

    //Sound when picking up stars
    public AudioClip pickupSound;

    //lose
    public AudioClip crashSound;

    //win
    public AudioClip winSound;

    // Rigidbody attached to the player.
    private Rigidbody rb;

    // Number of collected pickups.
    private int count;

    // Player movement input.
    private float movementX;
    private float movementY;

    // Prevents win and lose events from happening together.
    private bool gameEnded = false;

    // Player movement speed.
    public float speed = 0;

    // Number of pickups required to win.
    public int totalPickups = 8;

    // Text displaying the pickup count.
    public TextMeshProUGUI countText;

    // Text displaying "You Win!" or "You Lose!".
    public GameObject winTextObject;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        count = 0;

        // Hide the win/lose text at the beginning.
        winTextObject.SetActive(false);

        SetCountText();
    }

    void OnMove(InputValue movementValue)
    {
        // Stop accepting movement input after the game ends.
        if (gameEnded)
        {
            movementX = 0;
            movementY = 0;
            return;
        }

        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void FixedUpdate()
    {
        if (gameEnded)
            return;

        Vector3 movement = new Vector3(
            movementX,
            0.0f,
            movementY
        );

        rb.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (gameEnded)
            return;

        if (other.gameObject.CompareTag("PickUp"))
        {
            // Hide the collected pickup.
            other.gameObject.SetActive(false);
            if (soundEffectSource != null && pickupSound != null)
                    {
                        soundEffectSource.PlayOneShot(pickupSound);
                    }

            count++;

            SetCountText();
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count;

        if (count >= totalPickups && !gameEnded)
        {
            gameEnded = true;

            // Display the win message.
            TextMeshProUGUI winText =
                winTextObject.GetComponent<TextMeshProUGUI>();

            if (winText != null)
            {
                winText.text = "You Win!";
            }

            if (soundEffectSource != null && winSound!=null)
            {
                soundEffectSource.PlayOneShot(winSound);
            }

            winTextObject.SetActive(true);

            // Stop the player.
            movementX = 0;
            movementY = 0;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Remove all enemies.
            DestroyAllEnemies();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameEnded)
            return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            gameEnded = true;

            if (soundEffectSource != null && crashSound != null)
            {
                soundEffectSource.PlayOneShot(crashSound);
            }

            // Display the lose message.
            TextMeshProUGUI winText =
                winTextObject.GetComponent<TextMeshProUGUI>();

            if (winText != null)
            {
                winText.text = "You Lose!";
            }

            winTextObject.SetActive(true);

            // Stop the player instead of destroying it.
            movementX = 0f;
            movementY = 0f;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;

            enabled = false;
            

            Collider playerCollider = GetComponent<Collider>();

            if (playerCollider != null)
            {
                playerCollider.enabled = false;
            }
        }
    }

    private void DestroyAllEnemies()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            // Disable immediately to prevent another collision.
            enemy.SetActive(false);
            // Destroy at the end of the frame.
            Destroy(enemy);
        }
    }
}