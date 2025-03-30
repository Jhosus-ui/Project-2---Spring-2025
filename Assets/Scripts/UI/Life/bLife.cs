using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public enum PlayerState { Normal, Hit, Dead }

    public int maxHealth = 5;
    public float invulnerabilityTime = 1f;
    public Image[] healthImages;
    public Sprite fullHealthSprite;
    public Sprite emptyHealthSprite;
    public Animator animator;
    public string hitTrigger = "Hit";
    public string deathTrigger = "Death";
    public float hitAnimationTime = 0.5f;
    public PlayerManager playerManager;
    public Collider2D damageCollider;
    public AudioClip hitSound;
    public AudioClip deathSound;
    public string sceneOnDeath = "GameOver";

    public  int currentHealth;
    private PlayerState currentState = PlayerState.Normal;
    private bool isInvulnerable = false;
    private Coroutine invulnerabilityCoroutine;
    private Rigidbody2D rb;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        if (currentState == PlayerState.Dead || isInvulnerable) return;

        ChangeState(PlayerState.Hit);
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHealthUI();

        if (hitSound) audioSource.PlayOneShot(hitSound);
        if (invulnerabilityCoroutine != null) StopCoroutine(invulnerabilityCoroutine);
        invulnerabilityCoroutine = StartCoroutine(InvulnerabilityTimer());

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        ChangeState(PlayerState.Dead);
        if (deathSound) audioSource.PlayOneShot(deathSound);

        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        damageCollider.enabled = false;
        playerManager.movementEnabled = false;
        playerManager.attackEnabled = false;
        playerManager.climbingEnabled = false;

        Invoke(nameof(LoadDeathScene), 2f);
    }

    void LoadDeathScene() => SceneManager.LoadScene(sceneOnDeath);

    IEnumerator InvulnerabilityTimer()
    {
        isInvulnerable = true;
        damageCollider.enabled = false;
        yield return new WaitForSeconds(invulnerabilityTime);
        isInvulnerable = false;
        if (currentState != PlayerState.Dead) damageCollider.enabled = true;
    }

    void ChangeState(PlayerState newState)
    {
        if (currentState == PlayerState.Hit && playerManager != null)
        {
            playerManager.movementEnabled = true;
            playerManager.attackEnabled = true;
        }

        if (newState == PlayerState.Hit)
        {
            animator.SetTrigger(hitTrigger);
            playerManager.movementEnabled = false;
            playerManager.attackEnabled = false;
            StartCoroutine(ReturnToNormalAfterHit());
        }
        else if (newState == PlayerState.Dead)
        {
            animator.SetTrigger(deathTrigger);
        }

        currentState = newState;
    }

    IEnumerator ReturnToNormalAfterHit()
    {
        yield return new WaitForSeconds(hitAnimationTime);
        if (currentState == PlayerState.Hit && currentHealth > 0) ChangeState(PlayerState.Normal);
    }

    void UpdateHealthUI()
    {
        for (int i = 0; i < healthImages.Length; i++)
        {
            healthImages[i].sprite = i < currentHealth ? fullHealthSprite : emptyHealthSprite;
            healthImages[i].gameObject.SetActive(i < maxHealth);
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