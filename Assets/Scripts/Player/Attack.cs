using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackCooldown = 0.5f;
    public float swordAttackDuration = 0.5f;
    public float gunAttackDuration = 0.5f;
    public int swordDamage = 1; 
    public int bulletDamage = 1; 

    [Header("Sword Attack")]
    public Transform swordHitPoint; 
    public float swordHitRadius = 0.5f; 

    [Header("Gun Attack")]
    public Transform gunFirePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float bulletDistance = 10f;

    private float lastAttackTime = 0f;
    private PlayerMovement playerMovement;

    [Header("Ammo System")]
    public FabulasAmmo fabulasAmmo;

    [Header("Sound Effects")]
    public AudioClip swordSwingSound;
    public AudioClip gunShootSound;

    private AudioSource audioSource;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (!enabled) return;

        bool canAttack = Time.time > lastAttackTime + attackCooldown;
        bool isClimbing = (playerMovement.currentState == PlayerMovement.PlayerState.Escalar);

        if (!isClimbing && canAttack)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                lastAttackTime = Time.time;
                playerMovement.StartSwordAttack(swordAttackDuration);
                PerformSwordAttack();
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                if (fabulasAmmo != null && fabulasAmmo.CanShoot())
                {
                    lastAttackTime = Time.time;
                    playerMovement.StartGunAttack(gunAttackDuration);
                    fabulasAmmo.ConsumeCharge();
                }
                else
                {
                    // Opcional: Sonido o feedback de no tener cargas
                    Debug.Log("No hay cargas de fábula disponibles");
                }
            }
        }
    }

    // Método para realizar el ataque de espada
    private void PerformSwordAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(swordHitPoint.position, swordHitRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            ApplyDamage(enemy, swordDamage);
        }
        if (swordSwingSound != null)
        {
            audioSource.PlayOneShot(swordSwingSound);
        }
    }

    // Método para el disparo
    public void FireBullet()
    {
        if (gunFirePoint != null && bulletPrefab != null)
        {
            GameObject nuevaBala = Instantiate(bulletPrefab, gunFirePoint.position, Quaternion.identity);
            Bala balaScript = nuevaBala.GetComponent<Bala>();

            if (balaScript != null)
            {
                Vector2 dir = playerMovement.isFacingRight ? Vector2.right : Vector2.left;
                balaScript.Configurar(
                    dir,
                    bulletSpeed,
                    bulletDistance,
                    bulletDamage
                );

                // Rotación visual opcional (si tu sprite lo necesita)
                nuevaBala.transform.right = dir;

                if (gunShootSound != null)
                {
                    audioSource.PlayOneShot(gunShootSound);
                }
            }

        }
    }

    // Método común para aplicar daño
    private void ApplyDamage(Collider2D target, int damage)
    {
        EnemyHealth enemyHealth = target.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
        }
    }

    // Dibujar Gizmos para visualizar el área de daño de la espada
    private void OnDrawGizmosSelected()
    {
        if (swordHitPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(swordHitPoint.position, swordHitRadius);
        }
    }
}
