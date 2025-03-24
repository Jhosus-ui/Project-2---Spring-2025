using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Settings")]
    public bool movementEnabled = true; // Activa o desactiva el movimiento
    public bool climbingEnabled = true; // Activa o desactiva la escalada
    public bool attackEnabled = true; // Activa o desactiva el ataque

    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    private void Awake()
    {
        // Obtiene los componentes PlayerMovement y PlayerAttack del personaje
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();
    }

    private void Update()
    {
        // Habilita o deshabilita el movimiento según la variable movementEnabled
        if (playerMovement != null)
        {
            playerMovement.enabled = movementEnabled;
        }

        // Habilita o deshabilita el ataque según la variable attackEnabled
        if (playerAttack != null)
        {
            playerAttack.enabled = attackEnabled;
        }
    }
}