using UnityEngine;

public class EnemyPlatformerController : MonoBehaviour
{
    public enum EnemyState
    {
        Patrolling,
        Chasing,
        Attacking,
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
    private bool movingRight = true;
    private float attackTimer;
    private bool firstAttack = true; // Variable para controlar el primer ataque

    // Animator parameters as constants
    private const string SPEED_PARAMETER = "Speed";
    private const string ATTACK_TRIGGER = "Attack";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        patrolStartX = patrolAreaReference.position.x - patrolAreaSize.x / 2;
        patrolEndX = patrolAreaReference.position.x + patrolAreaSize.x / 2;
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
        float moveDirection = movingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(patrolSpeed * moveDirection, rb.linearVelocity.y);

        // Update animator speed
        animator.SetFloat(SPEED_PARAMETER, Mathf.Abs(rb.linearVelocity.x));

        // Cambiar dirección si se alcanza el límite de patrullaje
        if (movingRight && transform.position.x >= patrolEndX)
        {
            FlipDirection();
        }
        else if (!movingRight && transform.position.x <= patrolStartX)
        {
            FlipDirection();
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
                if (hitCollider.transform.position.x >= patrolStartX && hitCollider.transform.position.x <= patrolEndX)
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

        // Girar si es necesario
        if (shouldFaceRight != facingRight)
        {
            FlipDirection();
        }

        // Movimiento de persecución
        float directionToPlayer = shouldFaceRight ? 1 : -1;
        rb.linearVelocity = new Vector2(chaseSpeed * directionToPlayer, rb.linearVelocity.y);

        // Update animator speed
        animator.SetFloat(SPEED_PARAMETER, Mathf.Abs(rb.linearVelocity.x));

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
                if (hitCollider.transform.position.x >= patrolStartX && hitCollider.transform.position.x <= patrolEndX)
                {
                    playerDetected = true;
                    break;
                }
            }
        }

        if (!playerDetected || playerTransform.position.x < patrolStartX || playerTransform.position.x > patrolEndX)
        {
            currentState = EnemyState.Patrolling;
        }
    }

    void HandleAttacking()
    {
        attackTimer += Time.deltaTime;
        float currentCooldown = firstAttack ? initialAttackDelay : attackCooldown; // Usar initialAttackDelay para el primer ataque

        if (attackTimer >= currentCooldown)
        {
            // Verificar si hay un jugador en el rango de ataque usando un OverlapBox
            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
                attackPointReference.position,
                attackAreaSize,
                0f
            );

            bool playerInAttackRange = false;
            foreach (Collider2D hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    playerInAttackRange = true;
                    break;
                }
            }

            // Solo atacar si hay un jugador en el rango
            if (playerInAttackRange)
            {
                // Trigger attack animation
                animator.SetTrigger(ATTACK_TRIGGER);
                attackTimer = 0;
                firstAttack = false; // Después del primer ataque, cambiar a false
            }
        }

        // Regresar a persecución si jugador está fuera de rango
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > attackDistance) // Usar la nueva variable de distancia de ataque
        {
            currentState = EnemyState.Chasing;
            firstAttack = true; // Reiniciar para el próximo encuentro
        }
    }

    void FlipDirection()
    {
        // Cambiar dirección de facing
        facingRight = !facingRight;

        // Rotar el sprite
        transform.Rotate(0, 180, 0);
        movingRight = !movingRight;
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


