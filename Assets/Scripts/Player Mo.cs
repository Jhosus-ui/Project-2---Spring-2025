using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float doubleJumpForce = 8f;
    public float fallMultiplier = 2.5f;
    public float coyoteTime = 0.1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask platformerLayer;

    // Enum público para estados del jugador (accesible desde otros scripts)
    public enum PlayerState
    {
        Idle,
        Run,
        Jump,
        Fall,
        Subidita,
        Escalar,
        SwordAttack,
        GunAttack
    }

    [Header("State")]
    public PlayerState currentState;

    // Variables para referencias y control
    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool canDoubleJump;
    private float coyoteTimeCounter;
    private bool hasJumpedAfterFall;
    private bool isFacingRight = true;
    private bool isClimbing = false;

    // Variables para bloquear movimiento durante ataques
    private bool isAttacking = false;
    private float attackLockoutTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Si está atacando, controlar el lockout
        if (isAttacking)
        {
            if (Time.time >= attackLockoutTime)
            {
                isAttacking = false;
            }
            else
            {
                // Durante el ataque, permitir solo que caiga y se mueva (sin saltar ni cambiar animación)
                HandleGravity();
                float attackMoveInput = Input.GetAxis("Horizontal"); // Nombre diferente aquí
                rb.linearVelocity = new Vector2(attackMoveInput * moveSpeed * 0.5f, rb.linearVelocity.y);
                FlipCharacter(attackMoveInput);
                return;
            }
        }

        // Verificar si está en el suelo
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) ||
                     Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformerLayer);

        // Manejar Coyote Time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            hasJumpedAfterFall = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Procesar movimiento horizontal
        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Voltear el personaje según dirección de movimiento
        FlipCharacter(moveInput);

        // Manejar caída aumentada
        HandleGravity();

        // Manejar saltos
        HandleJumps();

        // Actualizar animaciones
        UpdateAnimationState(moveInput);
    }

    private void FlipCharacter(float moveInput)
    {
        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void HandleGravity()
    {
        if (rb.linearVelocity.y < 0 && !isClimbing)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    private void HandleJumps()
    {
        // No permitir saltos durante ataques
        if (isAttacking) return;

        // Salto normal
        if ((isGrounded || coyoteTimeCounter > 0) && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = true;
            coyoteTimeCounter = 0;
            ChangeState(PlayerState.Jump);
        }
        // Doble salto
        else if (!isGrounded && canDoubleJump && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
            canDoubleJump = false;
            ChangeState(PlayerState.Jump);
        }
        // Salto después de caer
        else if (!isGrounded && !hasJumpedAfterFall && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            hasJumpedAfterFall = true;
            ChangeState(PlayerState.Jump);
        }
    }

    private void UpdateAnimationState(float moveInput)
    {
        // No cambiar animaciones durante ataques
        if (isAttacking) return;

        // No cambiar animaciones durante Subidita (esperemos que termine)
        if (currentState == PlayerState.Subidita) return;

        // No cambiar animaciones si está escalando
        if (isClimbing) return;

        if (isGrounded)
        {
            if (Mathf.Abs(moveInput) > 0.1f)
            {
                ChangeState(PlayerState.Run);
            }
            else
            {
                ChangeState(PlayerState.Idle);
            }
        }
        else if (rb.linearVelocity.y < -0.1f)
        {
            ChangeState(PlayerState.Fall);
        }
        else if (rb.linearVelocity.y > 0.1f)
        {
            ChangeState(PlayerState.Jump);
        }
    }

    // Método mejorado para cambiar de estado
    public void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return;

        // Salir del estado anterior si es necesario
        ExitState(currentState);

        // Aplicar nuevo estado
        currentState = newState;

        // Configuración específica para el nuevo estado
        EnterState(currentState);
    }

    private void ExitState(PlayerState oldState)
    {
        // Acciones al salir de un estado
        switch (oldState)
        {
            case PlayerState.Escalar:
                animator.speed = 1f; // Asegurar que la velocidad de animación vuelve a la normalidad
                break;
        }
    }

    private void EnterState(PlayerState newState)
    {
        // Acciones al entrar en un nuevo estado
        switch (newState)
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
                // Configurar un temporizador para volver al estado normal después de X segundos
                Invoke("ResetAfterSubidita", 0.5f); // Ajusta el tiempo según la duración de tu animación
                break;
            case PlayerState.Escalar:
                animator.Play("Escalar");
                break;
            case PlayerState.SwordAttack:
                animator.Play("SwordAttack");
                break;
            case PlayerState.GunAttack:
                animator.Play("GunAttack");
                break;
        }
    }

    // Método para resetear después de Subidita
    private void ResetAfterSubidita()
    {
        if (currentState == PlayerState.Subidita)
        {
            // Volver al estado adecuado
            if (isGrounded)
            {
                ChangeState(Mathf.Abs(rb.linearVelocity.x) > 0.1f ? PlayerState.Run : PlayerState.Idle);
            }
            else
            {
                ChangeState(rb.linearVelocity.y < 0 ? PlayerState.Fall : PlayerState.Jump);
            }
        }
    }

    // Flip function
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // === Métodos públicos para interactuar con otros scripts ===

    // Iniciar ataque con espada
    public void StartSwordAttack(float duration)
    {
        if (isAttacking) return;

        isAttacking = true;
        attackLockoutTime = Time.time + duration;
        ChangeState(PlayerState.SwordAttack);
    }

    // Iniciar ataque con arma
    public void StartGunAttack(float duration)
    {
        if (isAttacking) return;

        isAttacking = true;
        attackLockoutTime = Time.time + duration;
        ChangeState(PlayerState.GunAttack);
    }

    // Método para cambiar al estado de Subidita
    public void StartSubidita()
    {
        ChangeState(PlayerState.Subidita);
    }

    // Método para cambiar al estado de Escalar
    public void StartEscalar()
    {
        isClimbing = true;
        ChangeState(PlayerState.Escalar);
    }

    // Método para volver al estado normal
    public void ResetState()
    {
        isClimbing = false;
        animator.speed = 1f; // Asegurar que la velocidad de animación vuelve a la normalidad

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
            animator.speed = 0;
        }
    }

    // Método para reanudar la animación de escalar
    public void ResumeEscalarAnimation()
    {
        if (currentState == PlayerState.Escalar)
        {
            animator.speed = 1;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}