using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configuraci�n")]
    public int maxHealth = 3;
    public float invulnerabilityTime = 0.5f;
    public float deathDelay = 2f;

    [Header("Referencias")]
    public Animator animator;
    public string hitTrigger = "Hit";
    public string deathTrigger = "Death";
    public Rigidbody2D rb; // A�adido para control f�sico

    public int currentHealth;
    private bool isDead = false;
    private Vector2 deathPosition; // Guardar� la posici�n al morir

    void Start()
    {
        currentHealth = maxHealth;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

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
        deathPosition = transform.position; // Captura la posici�n exacta

        // 1. Congelar f�sicamente el cuerpo
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // 2. Desactivar scripts (versi�n autom�tica)
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this && script != animator)
            {
                script.enabled = false;
            }
        }

        // 3. Reproducir animaci�n
        animator?.SetTrigger(deathTrigger);

        // 4. Destrucci�n despu�s de un delay
        Destroy(gameObject, deathDelay);
    }

    // Opcional: Mantener posici�n exacta durante la animaci�n
    void LateUpdate()
    {
        if (isDead)
        {
            transform.position = deathPosition;
        }
    }
}