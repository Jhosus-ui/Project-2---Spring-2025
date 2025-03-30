using UnityEngine;

public class Skeleton : MonoBehaviour
{
    //I didn't bother making four different codes for the enemies, it's the same,
    //just change some names for better evaluation, I think I complicated this... hahah
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
    public float initialAttackDelay = 1f;
    public Transform attackPointReference;
    public Vector2 attackAreaSize = new Vector2(2f, 2f);
    public float attackDistance = 1.5f;

    [Header("Movement Settings")]
    public float chaseSpeed = 3.5f;
    public bool facingRight = true;

    [Header("Sound Settings")]
    public AudioClip proximitySound;
    public AudioClip attackSound;
    public float proximitySoundDistance = 5f;
    public float proximitySoundInterval = 1f;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform playerTransform;
    private AudioSource audioSource;
    private float proximitySoundTimer;
    private bool isPlayerInProximity;

    private EnemyState currentState = EnemyState.Patrolling;

    private float patrolStartX;
    private float patrolEndX;
    private bool movingRight = true;
    private float attackTimer;
    private bool firstAttack = true;

    private const string SPEED_PARAMETER = "Speed";
    private const string ATTACK_TRIGGER = "Attack";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        patrolStartX = patrolAreaReference.position.x - patrolAreaSize.x / 2;
        patrolEndX = patrolAreaReference.position.x + patrolAreaSize.x / 2;
        attackTimer = 0f;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        proximitySoundTimer = 0f;
        isPlayerInProximity = false;
    }

    void Update()
    {
        CheckProximitySound();

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

    void CheckProximitySound()
    {
        if (playerTransform == null || proximitySound == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        bool nowInProximity = distanceToPlayer <= proximitySoundDistance;

        if (nowInProximity != isPlayerInProximity)
        {
            isPlayerInProximity = nowInProximity;
            proximitySoundTimer = 0f;

            if (!isPlayerInProximity && audioSource.isPlaying && audioSource.clip == proximitySound)
            {
                audioSource.Stop();
            }
        }

        if (isPlayerInProximity)
        {
            proximitySoundTimer += Time.deltaTime;
            if (proximitySoundTimer >= proximitySoundInterval)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.clip = proximitySound;
                    audioSource.Play();
                }
                proximitySoundTimer = 0f;
            }
        }
    }

    void HandlePatrolling()
    {
        float moveDirection = movingRight ? 1 : -1;
        rb.linearVelocity = new Vector2(patrolSpeed * moveDirection, rb.linearVelocity.y);

        animator.SetFloat(SPEED_PARAMETER, Mathf.Abs(rb.linearVelocity.x));

        if (movingRight && transform.position.x >= patrolEndX)
        {
            FlipDirection();
        }
        else if (!movingRight && transform.position.x <= patrolStartX)
        {
            FlipDirection();
        }

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
        bool shouldFaceRight = playerTransform.position.x > transform.position.x;

        if (shouldFaceRight != facingRight)
        {
            FlipDirection();
        }

        float directionToPlayer = shouldFaceRight ? 1 : -1;
        rb.linearVelocity = new Vector2(chaseSpeed * directionToPlayer, rb.linearVelocity.y);

        animator.SetFloat(SPEED_PARAMETER, Mathf.Abs(rb.linearVelocity.x));

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= attackDistance)
        {
            currentState = EnemyState.Attacking;
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat(SPEED_PARAMETER, 0);
        }

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
        float currentCooldown = firstAttack ? initialAttackDelay : attackCooldown;

        if (attackTimer >= currentCooldown)
        {
            animator.SetTrigger(ATTACK_TRIGGER);

            if (attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }

            firstAttack = false;
            attackTimer = 0f;
            currentState = EnemyState.AttackWindup;
        }
    }

    public void AnimationAttackHitEvent()
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

    public void AnimationAttackEnd()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        currentState = distanceToPlayer <= attackDistance ? EnemyState.Attacking : EnemyState.Chasing;
    }

    void FlipDirection()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
        movingRight = !movingRight;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (patrolAreaReference != null)
        {
            Gizmos.DrawWireCube(patrolAreaReference.position, patrolAreaSize);
        }

        Gizmos.color = Color.yellow;
        if (detectionPointReference != null)
        {
            Gizmos.DrawWireCube(detectionPointReference.position, detectionAreaSize);
        }

        Gizmos.color = Color.red;
        if (attackPointReference != null)
        {
            Gizmos.DrawWireCube(attackPointReference.position, attackAreaSize);
        }
    }
}