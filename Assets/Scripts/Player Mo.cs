using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Velocidad de movimiento horizontal
    public float jumpForce = 10f; // Fuerza del salto
    public float doubleJumpForce = 8f; // Fuerza del doble salto

    [Header("Ground Check")]
    public Transform groundCheck; // Objeto para verificar si está en el suelo
    public float groundCheckRadius = 0.2f; // Radio de detección del suelo
    public LayerMask groundLayer; // Capa que representa el suelo
    public LayerMask platformerLayer; // Capa que representa las plataformas (Platformer)

    private Rigidbody2D rb;
    private bool isGrounded; // Verifica si el personaje está en el suelo
    private bool isOnPlatformer; // Verifica si el personaje está en una plataforma Platformer
    private bool canDoubleJump; // Verifica si puede hacer doble salto

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Verifica si el personaje está en el suelo o en una plataforma Platformer
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isOnPlatformer = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformerLayer);

        // Movimiento horizontal
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Salto (desde el suelo o una plataforma Platformer)
        if ((isGrounded || isOnPlatformer) && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canDoubleJump = true; // Habilita el doble salto
        }
        // Doble salto
        else if (!isGrounded && !isOnPlatformer && canDoubleJump && Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
            canDoubleJump = false; // Deshabilita el doble salto después de usarlo
        }

        // Bajar a través de plataformas unidireccionales
        if ((isGrounded || isOnPlatformer) && Input.GetAxis("Vertical") < 0 && Input.GetButtonDown("Jump"))
        {
            Collider2D platformCollider = GetPlatformCollider();
            if (platformCollider != null && platformCollider.CompareTag("OneWayPlatform"))
            {
                StartCoroutine(DisableCollision(platformCollider));
            }
        }
    }

    private Collider2D GetPlatformCollider()
    {
        // Obtiene el collider de la plataforma debajo del personaje
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, platformerLayer);
        if (hit.collider != null)
        {
            return hit.collider;
        }
        return null;
    }

    private IEnumerator DisableCollision(Collider2D platformCollider)
    {
        // Ignora la colisión entre el personaje y la plataforma
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), platformCollider, true);

        // Espera un breve momento antes de reactivar la colisión
        yield return new WaitForSeconds(0.5f);

        // Reactiva la colisión
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), platformCollider, false);
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja el área de detección del suelo en el Editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}