using UnityEngine;

public class Plataformas : MonoBehaviour
{
    PlatformEffector2D pE2D;
    public bool LeftPlatform;
    public float upwardForce = 5f;

    // Agregar contador de tiempo para la acción de bajarse
    private float dropTimer = 0f;
    private float dropDelay = 0.5f;

    void Start()
    {
        pE2D = GetComponent<PlatformEffector2D>();
    }

    void Update()
    {
        // Si el jugador estaba tratando de bajar, pero ya pasó el tiempo
        if (LeftPlatform && Time.time > dropTimer)
        {
            // Restaurar la plataforma
            pE2D.rotationalOffset = 0;
            LeftPlatform = false;
            gameObject.layer = 6; // Ajusta según tu configuración de capas
        }

        if (Input.GetKeyDown(KeyCode.S) && !LeftPlatform)
        {
            pE2D.rotationalOffset = 180;
            LeftPlatform = true;
            gameObject.layer = 2; // Cambia a la capa "Ignore Raycast"

            // Configurar temporizador para restaurar automáticamente
            dropTimer = Time.time + dropDelay;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !LeftPlatform)
        {
            // Solo aplicar el impulso si el jugador viene desde abajo
            if (collision.relativeVelocity.y > 0)
            {
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();

                if (playerRb != null && playerMovement != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, upwardForce);
                    playerMovement.StartSubidita();
                }
            }
            else
            {
                // Si el jugador está parado sobre la plataforma, restaurar valores por seguridad
                pE2D.rotationalOffset = 0;
                LeftPlatform = false;
            }
        }
    }
}