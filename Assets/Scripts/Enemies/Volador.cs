using UnityEngine;

public class Volador : MonoBehaviour
{
    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Attacking,
        AttackWindup,
        Returning,
        Hit
    }

    [Header("Patrol Settings")]
    public float patrolSpeed = 2f;
    public Transform patrolAreaReference;
    public Vector2 patrolAreaSize = new Vector2(5f, 2f);

    [Header("Detection Settings")]
    public Transform detectionPointReference;
    public Vector2 detectionAreaSize = new Vector2(5f, 3f);

    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float initialAttackDelay = 1f; // Nueva variable para el tiempo del primer ataque
    public Transform attackPointReference;
    public Vector2 attackAreaSize = new Vector2(2f, 2f);
    public float attackDistance = 1.5f; // Nueva variable para la distancia de ataque

    [Header("Movement Settings")]
    public float chaseSpeed = 3.5f;
    public bool facingRight = true;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform playerTransform;

    private EnemyState currentState = EnemyState.Patrolling;

    private float patrolStartX;
    private float patrolEndX;
    private float patrolStartY;
    private float patrolEndY;
    private bool movingRight = true;
    private bool movingUp = true;
    private float attackTimer;
    private bool firstAttack = true; // Variable para controlar el primer ataque

    // Animator parameters as constants
    private const string SPEED_PARAMETER = "Speed";
    private const string ATTACK_TRIGGER = "Attack";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Establecer la gravedad a 0
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        patrolStartX = patrolAreaReference.position.x - patrolAreaSize.x / 2;
        patrolEndX = patrolAreaReference.position.x + patrolAreaSize.x / 2;
        patrolStartY = patrolAreaReference.position.y - patrolAreaSize.y / 2;
        patrolEndY = patrolAreaReference.position.y + patrolAreaSize.y / 2;
        attackTimer = 0f; // Inicialización
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrolling:
                HandlePatrolling();
                break;
            case EnemyState.Chasing:
                HandleChasing();
                break;
            case EnemyState.Attacking:
                HandleAttacking();
                break;
        }
    }

    void HandlePatrolling()
    {
        float moveDirectionX = movingRight ? 1 : -1;
        float moveDirectionY = movingUp ? 1 : -1;
        rb.linearVelocity = new Vector2(patrolSpeed * moveDirectionX, patrolSpeed * moveDirectionY);

        // Update animator speed
        animator.SetFloat(SPEED_PARAMETER, rb.linearVelocity.magnitude);

        // Cambiar dirección si se alcanza el límite de patrullaje en X
        if (movingRight && transform.position.x >= patrolEndX)
        {
            movingRight = false;
            FlipDirection();
        }
        else if (!movingRight && transform.position.x <= patrolStartX)
        {
            movingRight = true;
            FlipDirection();
        }

        // Cambiar dirección si se alcanza el límite de patrullaje en Y
        if (movingUp && transform.position.y >= patrolEndY)
        {
            movingUp = false;
        }
        else if (!movingUp && transform.position.y <= patrolStartY)
        {
            movingUp = true;
        }

        // Detectar jugador usando OverlapBox con tag
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            detectionPointReference.position,
            detectionAreaSize,
            0f
        );

        bool playerDetected = false;
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // Verificar si el jugador está dentro del área de patrullaje
                if (hitCollider.transform.position.x >= patrolStartX && hitCollider.transform.position.x <= patrolEndX &&
                    hitCollider.transform.position.y >= patrolStartY && hitCollider.transform.position.y <= patrolEndY)
                {
                    playerDetected = true;
                    break;
                }
            }
        }

        if (playerDetected)
        {
            currentState = EnemyState.Chasing;
        }
    }

    void HandleChasing()
    {
        // Determinar dirección hacia el jugador
        bool shouldFaceRight = playerTransform.position.x > transform.position.x;
        bool shouldMoveUp = playerTransform.position.y > transform.position.y;

        // Girar si es necesario
        if (shouldFaceRight != facingRight && Mathf.Abs(playerTransform.position.x - transform.position.x) > 0.1f)
        {
            FlipDirection();
        }

        // Movimiento de persecución
        float directionToPlayerX = shouldFaceRight ? 1 : -1;
        float directionToPlayerY = shouldMoveUp ? 1 : -1;
        rb.linearVelocity = new Vector2(chaseSpeed * directionToPlayerX, chaseSpeed * directionToPlayerY);

        // Update animator speed
        animator.SetFloat(SPEED_PARAMETER, rb.linearVelocity.magnitude);

        // Verificar rango de ataque
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackDistance) // Usar la nueva variable de distancia de ataque
        {
            currentState = EnemyState.Attacking;
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat(SPEED_PARAMETER, 0);
        }

        // Volver a patrullar si el jugador ya no está en el área de detección o fuera del área de patrullaje
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            detectionPointReference.position,
            detectionAreaSize,
            0f
        );

        bool playerDetected = false;
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // Verificar si el jugador está dentro del área de patrullaje
                if (hitCollider.transform.position.x >= patrolStartX && hitCollider.transform.position.x <= patrolEndX &&
                    hitCollider.transform.position.y >= patrolStartY && hitCollider.transform.position.y <= patrolEndY)
                {
                    playerDetected = true;
                    break;
                }
            }
        }

        if (!playerDetected || playerTransform.position.x < patrolStartX || playerTransform.position.x > patrolEndX ||
            playerTransform.position.y < patrolStartY || playerTransform.position.y > patrolEndY)
        {
            currentState = EnemyState.Patrolling;
        }
    }

    void HandleAttacking()
    {
        attackTimer += Time.deltaTime;
        float currentCooldown = firstAttack ? initialAttackDelay : attackCooldown;

        if (attackTimer >= currentCooldown)
        {
            // Activar animación primero
            animator.SetTrigger(ATTACK_TRIGGER);

            // Marcar que ya no es el primer ataque
            firstAttack = false;

            // Reiniciar timer después de activar la animación
            attackTimer = 0f;

            // Cambiar a un estado de "pre-ataque" o esperar a la animación
            currentState = EnemyState.AttackWindup;
        }
    }

    // Nuevo método para manejar el momento del impacto
    public void AnimationAttackHitEvent() // Llamado desde Animation Event
    {
        if (currentState != EnemyState.Attacking && currentState != EnemyState.AttackWindup)
            return;

        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            attackPointReference.position,
            attackAreaSize,
            0f
        );

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hitCollider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(1);
                }
                break;
            }
        }
    }

    // Nuevo método para cuando termina la animación
    public void AnimationAttackEnd() // Llamado desde Animation Event
    {
        // Verificar si el jugador sigue en rango
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        currentState = distanceToPlayer <= attackDistance ? EnemyState.Attacking : EnemyState.Chasing;
    }


    void FlipDirection()
    {
        // Cambiar dirección de facing
        facingRight = !facingRight;

        // Rotar el sprite
        transform.Rotate(0, 180, 0);
    }

    void OnDrawGizmos()
    {
        // Área de patrullaje
        Gizmos.color = Color.green;
        if (patrolAreaReference != null)
        {
            Gizmos.DrawWireCube(patrolAreaReference.position, patrolAreaSize);
        }

        // Rango de detección
        Gizmos.color = Color.yellow;
        if (detectionPointReference != null)
        {
            Gizmos.DrawWireCube(detectionPointReference.position, detectionAreaSize);
        }

        // Rango de ataque
        Gizmos.color = Color.red;
        if (attackPointReference != null)
        {
            Gizmos.DrawWireCube(attackPointReference.position, attackAreaSize);
        }
    }
}
