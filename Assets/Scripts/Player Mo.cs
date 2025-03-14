using UnityEngine;

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

    private Rigidbody2D rb;
    private bool isGrounded; // Verifica si el personaje está en el suelo
    private bool canDoubleJump; // Verifica si puede hacer doble salto

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Verifica si el personaje está en el suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Movimiento horizontal
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Salto
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = true; // Habilita el doble salto
        }
        // Doble salto
        else if (!isGrounded && canDoubleJump && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
            canDoubleJump = false; // Deshabilita el doble salto después de usarlo
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