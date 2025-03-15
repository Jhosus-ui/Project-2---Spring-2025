using UnityEngine;

public class LedgeDetector : MonoBehaviour
{
    [Header("Ledge Detection")]
    public Transform ledgeCheck; // Punto desde donde se detecta el borde
    public float ledgeCheckRadius = 0.1f; // Radio de detección del borde
    public LayerMask platformLayer; // Capa de las plataformas

    private bool isLedgeDetected; // Verifica si se detectó un borde
    private Collider2D ledgeCollider; // Collider del borde detectado

    private void Update()
    {
        // Detecta si hay un borde cerca
        isLedgeDetected = Physics2D.OverlapCircle(ledgeCheck.position, ledgeCheckRadius, platformLayer);

        if (isLedgeDetected)
        {
            ledgeCollider = Physics2D.OverlapCircle(ledgeCheck.position, ledgeCheckRadius, platformLayer);
        }
        else
        {
            ledgeCollider = null;
        }
    }

    public bool IsLedgeDetected()
    {
        return isLedgeDetected;
    }

    public Collider2D GetLedgeCollider()
    {
        return ledgeCollider;
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja el área de detección del borde en el Editor
        if (ledgeCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(ledgeCheck.position, ledgeCheckRadius);
        }
    }
}