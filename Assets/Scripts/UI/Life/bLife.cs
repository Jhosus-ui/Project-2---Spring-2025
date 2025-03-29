using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public enum PlayerState
    {
        Normal,
        Hit,
        Dead
    }

    [Header("Health Settings")]
    public int maxHealth = 5;
    public float invulnerabilityTime = 1f;
    public Image[] healthImages;
    public Sprite fullHealthSprite;
    public Sprite emptyHealthSprite;

    [Header("Hit Animation")]
    public Animator animator;
    public string hitTrigger = "Hit";
    public string deathTrigger = "Death"; 
    public float hitAnimationTime = 0.5f;

    [Header("References")]
    public PlayerManager playerManager;
    public Collider2D damageCollider;

    public int currentHealth;
    private PlayerState currentState = PlayerState.Normal;
    private bool isInvulnerable = false;
    private Coroutine invulnerabilityCoroutine;
    private Rigidbody2D rb;

    [Header("Sound Effects")]
    public AudioClip hitSound;
    public AudioClip deathSound;

    private AudioSource audioSource;


    void Start()
    {
        // ... (código existente)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void UpdateHealthUI()
    {
        for (int i = 0; i < healthImages.Length; i++)
        {
            if (i < currentHealth)
            {
                healthImages[i].sprite = fullHealthSprite;
            }
            else
            {
                healthImages[i].sprite = emptyHealthSprite;
            }

            healthImages[i].gameObject.SetActive(i < maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentState == PlayerState.Dead || isInvulnerable) return;

        ChangeState(PlayerState.Hit);
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        UpdateHealthUI();

        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (invulnerabilityCoroutine != null)
        {
            StopCoroutine(invulnerabilityCoroutine);
        }
        invulnerabilityCoroutine = StartCoroutine(InvulnerabilityTimer());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator InvulnerabilityTimer()
    {
        isInvulnerable = true;
        if (damageCollider != null)
        {
            damageCollider.enabled = false;
        }

        yield return new WaitForSeconds(invulnerabilityTime);

        isInvulnerable = false;
        if (damageCollider != null && currentState != PlayerState.Dead)
        {
            damageCollider.enabled = true;
        }
    }

    void Die()
    {
        ChangeState(PlayerState.Dead);
        Debug.Log("Player has died!");

        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);

        }
    }

        void ChangeState(PlayerState newState)
    {
        switch (currentState)
        {
            case PlayerState.Hit:
                if (playerManager != null)
                {
                    playerManager.movementEnabled = true;
                    playerManager.attackEnabled = true;
                }
                break;
        }

        switch (newState)
        {
            case PlayerState.Hit:
                if (animator != null)
                {
                    animator.SetTrigger(hitTrigger);
                }

                if (playerManager != null)
                {
                    playerManager.movementEnabled = false;
                    playerManager.attackEnabled = false;
                }

                StartCoroutine(ReturnToNormalAfterHit());
                break;

            case PlayerState.Dead:
                // Activar animación de muerte usando el trigger específico
                if (animator != null)
                {
                    animator.SetTrigger(deathTrigger);
                }

                // Congelar al jugador
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                    rb.simulated = false; // Desactiva la física sin cambiar isKinematic
                }

                // Desactivar controles permanentemente
                if (playerManager != null)
                {
                    playerManager.movementEnabled = false;
                    playerManager.attackEnabled = false;
                    playerManager.climbingEnabled = false;
                }

                // Desactivar colliders
                if (damageCollider != null)
                {
                    damageCollider.enabled = false;
                }
                break;
        }

        currentState = newState;
    }

    IEnumerator ReturnToNormalAfterHit()
    {
        yield return new WaitForSeconds(hitAnimationTime);

        if (currentState == PlayerState.Hit && currentHealth > 0)
        {
            ChangeState(PlayerState.Normal);
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        UpdateHealthUI();
    }
}

