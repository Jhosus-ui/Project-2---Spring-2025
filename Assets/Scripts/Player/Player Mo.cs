using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{

    //I don't know if I did the enums completely right,
    //but this is what I can do and with YouTube tutorials included hahah
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

    [Header("Sound Effects")]
    public AudioClip jumpSound;
    public AudioClip footstepSound;

    private AudioSource audioSource;
    private bool isPlayingFootstep = false;

    public enum PlayerState
    {
        Idle,
        Run,
        Jump,
        Fall,
        Subidita,
        Escalar,
        SwordAttack,
        GunAttack,
        //Roll,
        //Parry
        // Talvez el sistema de estados no es lo mio, pero recuerda esto josue;
        // siempre se puede mejorar y aprender más
    }   //Y ahora saber como explicarias esto en Ingles ;(

    [Header("State")]
    public PlayerState currentState;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded;
    private bool canDoubleJump;
    private float coyoteTimeCounter;
    private bool hasJumpedAfterFall;
    public bool isFacingRight = true;
    private bool isClimbing = false;

    public bool isAttacking = false;
    private float attackLockoutTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (isAttacking)
        {
            if (Time.time >= attackLockoutTime)
            {
                isAttacking = false;
            }
            else
            {
                HandleGravity();
                float attackMoveInput = Input.GetAxis("Horizontal");
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

        float moveInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        FlipCharacter(moveInput);
        HandleGravity();
        HandleJumps();
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
        if (isAttacking) return;

        // Salto normal
        if ((isGrounded || coyoteTimeCounter > 0) && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            canDoubleJump = true;
            coyoteTimeCounter = 0;
            ChangeState(PlayerState.Jump);

            if (jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
        // Doble salto
        else if (!isGrounded && canDoubleJump && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
            canDoubleJump = false;
            hasJumpedAfterFall = true;
            ChangeState(PlayerState.Jump);

            if (jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
        // Salto después de caer
        else if (!isGrounded && !hasJumpedAfterFall && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            hasJumpedAfterFall = true;
            ChangeState(PlayerState.Jump);
            if (jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }

    private void UpdateAnimationState(float moveInput)
    {
        bool isMoving = isGrounded && Mathf.Abs(moveInput) > 0.1f;

        // Reproducir sonido al caminar
        if (isMoving)
        {
            if (footstepSound != null && !isPlayingFootstep)
            {
                audioSource.loop = true;
                audioSource.clip = footstepSound;
                audioSource.Play();
                isPlayingFootstep = true;
            }
        }
        // Detener sonido cuando dejamos de movernos
        else if (!isMoving && isPlayingFootstep)
        {
            isPlayingFootstep = false;
            audioSource.Stop();
        }

        if (isAttacking) return;
        if (currentState == PlayerState.Subidita) return;
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
    public void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return;

        ExitState(currentState);
        currentState = newState;
        EnterState(currentState);
    }

    private void ExitState(PlayerState oldState)
    {
        switch (oldState)
        {
            case PlayerState.Escalar:
                animator.speed = 1f;
                break;
        }
    }

    private void EnterState(PlayerState newState)
    {
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
                Invoke("ResetAfterSubidita", 0.5f);
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
                //case PlayerState.Roll:
                //    animator.Play("Roll");
                //    break;
                //case PlayerState.Parry:
                //    animator.Play("Parry");
                //    break;
        }

    }
    // Método para resetear después de Subidita
    private void ResetAfterSubidita()
    {
        if (currentState == PlayerState.Subidita)
        {
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
        animator.speed = 1f;

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
