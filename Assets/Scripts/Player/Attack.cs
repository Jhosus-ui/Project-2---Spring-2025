using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackCooldown = 0.5f;
    public float swordAttackDuration = 0.5f;
    public float gunAttackDuration = 0.5f;

    [Header("Sword Attack")]
    [Tooltip("Arrastra aquí el Collider2D que detectará el daño de la espada")]
    public Collider2D swordHitbox; // Renombrado para mayor claridad

    [Header("Gun Attack")]
    public Transform gunFirePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float bulletDistance = 10f;

    private float lastAttackTime = 0f;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        DisableSwordHitbox(); // Asegurarse que empiece desactivado
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
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                lastAttackTime = Time.time;
                playerMovement.StartGunAttack(gunAttackDuration);
            }
        }
    }

    // Llamado desde evento de animación cuando el ataque debe causar daño
    public void EnableSwordHitbox()
    {
        if (swordHitbox != null)
            swordHitbox.enabled = true;
    }

    // Llamado desde evento de animación cuando el ataque termina
    public void DisableSwordHitbox()
    {
        if (swordHitbox != null)
            swordHitbox.enabled = false;
    }

    // Llamado desde evento de animación para el disparo
    // Método FireBullet actualizado
    public void FireBullet()
    {
        if (gunFirePoint != null && bulletPrefab != null)
        {
            // 1. Calcular dirección basada en isFacingRight
            Vector2 direction = playerMovement.isFacingRight ? Vector2.right : Vector2.left;

            // 2. Instanciar la bala con la rotación correcta
            GameObject bullet = Instantiate(
                bulletPrefab,
                gunFirePoint.position,
                Quaternion.FromToRotation(Vector2.right, direction) // Rotación explícita
            );

            // 3. Configurar velocidad (el signo ya no es necesario)
            Bala balaScript = bullet.GetComponent<Bala>();
            if (balaScript != null)
            {
                balaScript.speed = bulletSpeed; // Usamos valor absoluto
                balaScript.direction = direction; // Nueva variable en Bala.cs
            }
        }
    }
}
