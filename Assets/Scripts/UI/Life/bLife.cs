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
    public Image[] healthImages; // Array de im�genes que representan la vida (de lleno a vac�o)
    public Sprite fullHealthSprite;
    public Sprite emptyHealthSprite;

    [Header("Hit Animation")]
    public Animator animator;
    public string hitTrigger = "Hit";
    public float hitAnimationTime = 0.5f;

    [Header("References")]
    public PlayerManager playerManager;
    public Collider2D damageCollider; // Collider que detecta da�o

    private int currentHealth;
    private PlayerState currentState = PlayerState.Normal;
    private bool isInvulnerable = false;
    private Coroutine invulnerabilityCoroutine;

    // Animator parameters
    private const string HIT_STATE = "Hit";

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider2D>();
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

            // Activar/desactivar im�genes seg�n el m�ximo de vida
            healthImages[i].gameObject.SetActive(i < maxHealth);
        }

        // Actualizar par�metro de animator si es necesario
        if (animator != null)
        {
           
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentState == PlayerState.Dead || isInvulnerable) return;

        // Cambiar estado a Hit
        ChangeState(PlayerState.Hit);

        // Reducir vida
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        UpdateHealthUI();

        // Iniciar invulnerabilidad
        if (invulnerabilityCoroutine != null)
        {
            StopCoroutine(invulnerabilityCoroutine);
        }
        invulnerabilityCoroutine = StartCoroutine(InvulnerabilityTimer());

        // Verificar si el jugador muri�
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
        // Aqu� puedes agregar l�gica adicional para la muerte del jugador
        Debug.Log("Player has died!");
    }

    void ChangeState(PlayerState newState)
    {
        // Exit state logic
        switch (currentState)
        {
            case PlayerState.Hit:
                // Reactivar movimiento despu�s del golpe
                if (playerManager != null)
                {
                    playerManager.movementEnabled = true;
                    playerManager.attackEnabled = true;
                }
                break;
        }

        // Enter state logic
        switch (newState)
        {
            case PlayerState.Hit:
                // Activar animaci�n de golpe
                if (animator != null)
                {
                    animator.SetTrigger(hitTrigger);
                }

                // Desactivar movimiento durante el golpe
                if (playerManager != null)
                {
                    playerManager.movementEnabled = false;
                    playerManager.attackEnabled = false;
                }

                // Volver al estado normal despu�s de la animaci�n
                StartCoroutine(ReturnToNormalAfterHit());
                break;

            case PlayerState.Dead:
                // Desactivar controles permanentemente
                if (playerManager != null)
                {
                    playerManager.movementEnabled = false;
                    playerManager.attackEnabled = false;
                    playerManager.climbingEnabled = false;
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

    // M�todo para curar al jugador
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
    }

    // M�todo para aumentar la vida m�xima
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount;
        UpdateHealthUI();
    }
}