using UnityEngine;

public class Plataformas : MonoBehaviour
{
    PlatformEffector2D pE2D;

    public bool LeftPlatform;

    // Fuerza de impulso para subir a la plataforma
    public float upwardForce = 5f;

    void Start()
    {
        pE2D = GetComponent<PlatformEffector2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && !LeftPlatform)
        {
            pE2D.rotationalOffset = 180;
            LeftPlatform = true;
            gameObject.layer = 2; // Cambia a la capa "Ignore Raycast"
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si el personaje toca la plataforma desde abajo y LeftPlatform es false
        if (collision.gameObject.CompareTag("Player") && !LeftPlatform)
        {
            // Verifica si el personaje está tocando la plataforma desde abajo
            if (collision.relativeVelocity.y > 0)
            {
                // Aplica un impulso hacia arriba al personaje
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, upwardForce);
                }
            }
        }

        // Restablece la plataforma
        pE2D.rotationalOffset = 0;
        LeftPlatform = false;
        gameObject.layer = 6 | 7; // Restablece la capa original (ajusta según tus Layers)
    }
}