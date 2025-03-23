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

    [Header("Animation")]
    public Animator animator; // Referencia al Animator

    private Rigidbody2D rb;
    private bool isGrounded; // Verifica si el personaje está en el suelo
    private bool canDoubleJump; // Verifica si puede hacer doble salto
    private float coyoteTimeCounter; // Contador para el Coyote Time
    private bool hasJumpedAfterFall; // Verifica si el jugador ha saltado después de caer
    private bool isFacingRight = true; // Dirección en la que mira el jugador
    private bool isClimbing = false; // Indica si el jugador está escalando

    // Estados del jugador
    private enum PlayerState
    {
        Idle,
        Run,
        Jump,
        Fall,
        Subidita,
        Escalar
    }
    private PlayerState currentState;

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

        // Flip X: Cambia la dirección del jugador según el movimiento
        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
        }

        // Aceleración de caída: Aumenta la velocidad de caída progresivamente
        if (rb.linearVelocity.y < 0 && !isClimbing)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        // Salto normal
        if ((isGrounded || coyoteTimeCounter > 0) && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = true; // Habilita el doble salto
            coyoteTimeCounter = 0; // Desactiva el Coyote Time después de saltar
            ChangeState(PlayerState.Jump); // Cambia al estado de salto
        }
        // Doble salto
        else if (!isGrounded && canDoubleJump && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
            canDoubleJump = false; // Deshabilita el doble salto después de usarlo
            ChangeState(PlayerState.Jump); // Cambia al estado de salto
        }

        // Salto después de caer: Permite un salto si el jugador no ha saltado después de caer
        if (!isGrounded && !hasJumpedAfterFall && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            hasJumpedAfterFall = true; // Marca que el jugador ha saltado después de caer
            ChangeState(PlayerState.Jump); // Cambia al estado de salto
        }

        // Cambio de estado según el movimiento
        if (isGrounded && !isClimbing)
        {
            if (Mathf.Abs(moveInput) > 0.1f)
            {
                ChangeState(PlayerState.Run); // Cambia al estado de correr
            }
            else
            {
                ChangeState(PlayerState.Idle); // Cambia al estado de idle
            }
        }
        else if (rb.linearVelocity.y < 0 && !isClimbing)
        {
            ChangeState(PlayerState.Fall); // Cambia al estado de caída
        }
    }

    // Cambia el estado del jugador y actualiza las animaciones
    private void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return; // No hacer nada si el estado no cambia

        currentState = newState;

        // Actualiza las animaciones según el estado
        switch (currentState)
        {
            case PlayerState.Idle:
                animator.Play("Idle");
                break;
            case PlayerState.Run:
                animator.Play("Run");
                break;
            case PlayerState.Jump:
                animator.Play("Jump");
                break;
            case PlayerState.Fall:
                animator.Play("Fall");
                break;
            case PlayerState.Subidita:
                animator.Play("Subidita");
                break;
            case PlayerState.Escalar:
                animator.Play("Escalar");
                break;
        }
    }

    // Cambia la dirección del jugador (Flip X)
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // Método para cambiar al estado de Subidita
    public void StartSubidita()
    {
        ChangeState(PlayerState.Subidita);
    }

    // Método para cambiar al estado de Escalar
    public void StartEscalar()
    {
        isClimbing = true; // Marca que el jugador está escalando
        ChangeState(PlayerState.Escalar);
    }

    // Método para volver al estado normal (Idle, Run, etc.)
    public void ResetState()
    {
        isClimbing = false; // Marca que el jugador ya no está escalando

        if (isGrounded)
        {
            ChangeState(Mathf.Abs(rb.linearVelocity.x) > 0.1f ? PlayerState.Run : PlayerState.Idle);
        }
        else
        {
            ChangeState(rb.linearVelocity.y < 0 ? PlayerState.Fall : PlayerState.Jump);
        }
    }

    // Método para congelar la animación de escalar
    public void FreezeEscalarAnimation()
    {
        if (currentState == PlayerState.Escalar)
        {
            animator.speed = 0; // Congela la animación
        }
    }

    // Método para reanudar la animación de escalar
    public void ResumeEscalarAnimation()
    {
        if (currentState == PlayerState.Escalar)
        {
            animator.speed = 1; // Reanuda la animación
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