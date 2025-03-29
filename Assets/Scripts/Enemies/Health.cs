using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configuración")]
    public int maxHealth = 3;
    public float invulnerabilityTime = 0.5f;
    public float deathDelay = 2f;

    [Header("Referencias")]
    public Animator animator;
    public string hitTrigger = "Hit";
    public string deathTrigger = "Death";
    public Rigidbody2D rb; // Añadido para control físico

    [Header("Sound Effects")]
    public AudioClip hitSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    public int currentHealth;
    private bool isDead = false;
    private Vector2 deathPosition; // Guardará la posición al morir

    void Start()
    {
        currentHealth = maxHealth;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // Reproducir sonido de golpe
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            animator?.SetTrigger(hitTrigger);
        }
    }

    private void Die()
    {
        isDead = true;
        deathPosition = transform.position; // Captura la posición exacta

        // Reproducir sonido de muerte
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        // 1. Congelar físicamente el cuerpo
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // 2. Desactivar scripts (versión automática)
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this && script != animator)
            {
                script.enabled = false;
            }
        }

        // 3. Reproducir animación
        animator?.SetTrigger(deathTrigger);

        // 4. Destrucción después de un delay
        Destroy(gameObject, deathDelay);
    }

    // Opcional: Mantener posición exacta durante la animación
    void LateUpdate()
    {
        if (isDead)
        {
            transform.position = deathPosition;
        }
    }
}
