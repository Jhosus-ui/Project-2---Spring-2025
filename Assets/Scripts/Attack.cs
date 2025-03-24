using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackCooldown = 0.5f;
    public float swordAttackDuration = 0.5f;
    public float gunAttackDuration = 0.5f;

    private float lastAttackTime = 0f;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (!enabled) return;

        // Verificar si ha pasado el cooldown
        bool canAttack = Time.time > lastAttackTime + attackCooldown;

        // Solo permitir ataques si no está escalando
        bool isClimbing = (playerMovement.currentState == PlayerMovement.PlayerState.Escalar);

        if (!isClimbing && canAttack)
        {
            // Ataque con espada
            if (Input.GetButtonDown("Fire1"))
            {
                lastAttackTime = Time.time;
                playerMovement.StartSwordAttack(swordAttackDuration);
            }

            // Ataque con arma
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                lastAttackTime = Time.time;
                playerMovement.StartGunAttack(gunAttackDuration);
            }
        }
    }
}