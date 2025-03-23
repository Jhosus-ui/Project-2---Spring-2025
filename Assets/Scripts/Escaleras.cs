using UnityEngine;

public class Ladder : MonoBehaviour
{
    [Header("Ladder Settings")]
    public float climbSpeed = 5f; // Velocidad de escalada

    private Rigidbody2D playerRb; // Referencia al Rigidbody2D del jugador
    private bool isOnLadder = false; // Indica si el jugador está en la escalera

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el jugador entra en el collider de la escalera
        if (collision.CompareTag("Player"))
        {
            playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                isOnLadder = true;
                playerRb.gravityScale = 0; // Desactiva la gravedad
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0); // Detén el movimiento vertical

                // Cambia al estado de Escalar
                PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.StartEscalar();
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Verifica si el jugador está dentro del collider de la escalera
        if (collision.CompareTag("Player") && isOnLadder)
        {
            // Obtén la entrada vertical (W o S)
            float verticalInput = Input.GetAxisRaw("Vertical"); // Usamos GetAxisRaw para evitar valores pequeños

            // Mueve al jugador verticalmente solo si hay entrada
            if (verticalInput != 0)
            {
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, verticalInput * climbSpeed);

                // Reanuda la animación de escalar
                PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.ResumeEscalarAnimation();
                }
            }
            else
            {
                // Si no hay entrada, detén el movimiento vertical y congela la animación
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0);

                // Congela la animación de escalar
                PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    playerMovement.FreezeEscalarAnimation();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Verifica si el jugador sale del collider de la escalera
        if (collision.CompareTag("Player") && isOnLadder)
        {
            isOnLadder = false;
            if (playerRb != null)
            {
                playerRb.gravityScale = 1; // Reactiva la gravedad
            }

            // Vuelve al estado normal y reanuda la animación
            PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.ResetState();
                playerMovement.ResumeEscalarAnimation();
            }
        }
    }
}