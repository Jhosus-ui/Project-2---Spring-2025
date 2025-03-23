using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public bool attackEnabled = true; // Activa o desactiva el ataque
    public KeyCode gunAttackKey = KeyCode.Q; // Tecla para el ataque con arma
    public float attackCooldown = 0.5f; // Tiempo entre ataques

    // Referencias a los componentes
    private Animator animator; // Referencia al Animator
    private PlayerMovement playerMovement; // Referencia al movimiento del jugador
    private float lastAttackTime = 0f; // Tiempo del último ataque

    // Parámetros del Animator
    private const string IS_ATTACK_1 = "isAttack1"; // Parámetro para el ataque con espada
    private const string IS_ATTACK_2 = "isAttack2"; // Parámetro para el ataque con arma

    private void Awake()
    {
        // Obtiene los componentes Animator y PlayerMovement del jugador
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (!attackEnabled) return; // Si el ataque está desactivado, no hacer nada

        // Verifica si el jugador está atacando con la espada (Fire1)
        if (Input.GetButtonDown("Fire1") && Time.time > lastAttackTime + attackCooldown)
        {
            PerformAttack(IS_ATTACK_1);
        }

        // Verifica si el jugador está atacando con el arma (Tecla Q)
        if (Input.GetKeyDown(gunAttackKey) && Time.time > lastAttackTime + attackCooldown)
        {
            PerformAttack(IS_ATTACK_2);
        }
    }

    // Método para realizar un ataque
    private void PerformAttack(string attackParameter)
    {
        if (animator.GetBool(IS_ATTACK_1) || animator.GetBool(IS_ATTACK_2)) return; // Si ya está atacando, no hacer nada

        lastAttackTime = Time.time; // Registra el tiempo del último ataque

        // Desactiva el movimiento si es un ataque con arma
        if (attackParameter == IS_ATTACK_2 && playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Activa el parámetro correspondiente en el Animator
        animator.SetBool(attackParameter, true);

        // Aquí puedes agregar lógica adicional para el ataque (daño, sonido, etc.)
        Debug.Log("Ataque realizado: " + attackParameter);
    }

    // Método para finalizar el ataque (llamado desde la animación o manualmente)
    public void FinishAttack()
    {
        // Desactiva ambos parámetros de ataque en el Animator
        animator.SetBool(IS_ATTACK_1, false);
        animator.SetBool(IS_ATTACK_2, false);

        // Reactiva el movimiento si fue desactivado
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }
}