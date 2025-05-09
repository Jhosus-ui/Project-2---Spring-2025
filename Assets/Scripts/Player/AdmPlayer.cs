using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Settings")]
    public bool movementEnabled = true; 
    public bool climbingEnabled = true; 
    public bool attackEnabled = true; 

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
        // Habilita o deshabilita el movimiento seg�n la variable movementEnabled
        if (playerMovement != null)
        {
            playerMovement.enabled = movementEnabled;
        }

        // Habilita o deshabilita el ataque seg�n la variable attackEnabled
        if (playerAttack != null)
        {
            playerAttack.enabled = attackEnabled;
        }
    }
}