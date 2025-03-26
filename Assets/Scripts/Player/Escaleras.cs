using UnityEngine;

public class Ladder : MonoBehaviour
{
    [Header("Ladder Settings")]
    public float climbSpeed = 5f;

    private Rigidbody2D playerRb;
    private bool isOnLadder = false;
    private PlayerMovement playerMovement;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerRb = collision.GetComponent<Rigidbody2D>();
            playerMovement = collision.GetComponent<PlayerMovement>();

            if (playerRb != null && playerMovement != null)
            {
                isOnLadder = true;
                playerRb.gravityScale = 0;
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0);
                playerMovement.StartEscalar();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isOnLadder && playerMovement != null)
        {
            float verticalInput = Input.GetAxisRaw("Vertical");

            if (verticalInput != 0)
            {
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, verticalInput * climbSpeed);
                playerMovement.ResumeEscalarAnimation();
            }
            else
            {
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0);
                playerMovement.FreezeEscalarAnimation();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isOnLadder)
        {
            isOnLadder = false;

            if (playerRb != null)
            {
                playerRb.gravityScale = 1;
            }

            if (playerMovement != null)
            {
                // Asegurarnos de que el estado y la animación vuelvan a la normalidad
                playerMovement.ResetState();
            }
        }
    }
}