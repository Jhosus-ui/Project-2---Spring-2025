using UnityEngine;
using System.Collections;

public class OneWayPlatform : MonoBehaviour
{
    [Header("Platform Settings")]
    public float disableCollisionTime = 0.5f; // Tiempo que la colisi�n estar� desactivada

    private Collider2D platformCollider;

    private void Awake()
    {
        platformCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica si el objeto que colisiona es el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            // Si el jugador est� por encima de la plataforma, permite que pase a trav�s
            if (collision.relativeVelocity.y <= 0)
            {
                Physics2D.IgnoreCollision(collision.collider, platformCollider, true);
                StartCoroutine(EnableCollision(collision.collider));
            }
        }
    }

    private IEnumerator EnableCollision(Collider2D playerCollider)
    {
        // Espera un breve momento antes de reactivar la colisi�n
        yield return new WaitForSeconds(disableCollisionTime);

        // Reactiva la colisi�n
        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }
}