using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f; // Velocidad de movimiento horizontal
    public float jumpForce = 10f; // Fuerza del salto
    public float doubleJumpForce = 8f; // Fuerza del doble salto
    public float fallMultiplier = 2.5f; // Multiplicador de aceleración de caída
    public float coyoteTime = 0.1f; // Tiempo de gracia para saltar después de dejar el suelo

    [Header("Ground Check")]
    public Transform groundCheck; // Objeto para verificar si está en el suelo
    public float groundCheckRadius = 0.2f; // Radio de detección del suelo
    public LayerMask groundLayer; // Capa que representa el suelo
    public LayerMask platformerLayer; // Capa que representa las plataformas

    private Rigidbody2D rb;
    private bool isGrounded; // Verifica si el personaje está en el suelo
    private bool canDoubleJump; // Verifica si puede hacer doble salto
    private float coyoteTimeCounter; // Contador para el Coyote Time
    private bool hasJumpedAfterFall; // Verifica si el jugador ha saltado después de caer

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Verifica si el personaje está en el suelo o en una plataforma
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) ||
                     Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformerLayer);

        // Coyote Time: Permite saltar un poco después de dejar el suelo
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime; // Reinicia el contador de Coyote Time
            hasJumpedAfterFall = false; // Reinicia el estado de salto después de caer
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Reduce el contador de Coyote Time
        }

        // Movimiento horizontal
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Aceleración de caída: Aumenta la velocidad de caída progresivamente
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        // Salto normal
        if ((isGrounded || coyoteTimeCounter > 0) && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = true; // Habilita el doble salto
            coyoteTimeCounter = 0; // Desactiva el Coyote Time después de saltar
        }
        // Doble salto
        else if (!isGrounded && canDoubleJump && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
            canDoubleJump = false; // Deshabilita el doble salto después de usarlo
        }

        // Salto después de caer: Permite un salto si el jugador no ha saltado después de caer
        if (!isGrounded && !hasJumpedAfterFall && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            hasJumpedAfterFall = true; // Marca que el jugador ha saltado después de caer
        }
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