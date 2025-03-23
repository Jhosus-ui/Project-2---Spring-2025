using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Settings")]
    public bool movementEnabled = true; // Activa o desactiva el movimiento
    public bool climbingEnabled = true; // Activa o desactiva la escalada

    private PlayerMovement playerMovement;

    private void Awake()
    {
        // Obtiene el componente PlayerMovement del personaje
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // Habilita o deshabilita el movimiento según la variable movementEnabled
        if (playerMovement != null)
        {
            playerMovement.enabled = movementEnabled;
        }
    }
}