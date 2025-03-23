using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target; // Objeto que la cámara seguirá (normalmente el jugador)
    public float smoothSpeed = 0.125f; // Suavizado del movimiento de la cámara
    public Vector3 offset; // Desplazamiento de la cámara respecto al objetivo

    [Header("Camera Bounds")]
    public bool useBounds = false; // Activar límites de la cámara
    public Vector2 minBounds; // Límite mínimo (esquina inferior izquierda)
    public Vector2 maxBounds; // Límite máximo (esquina superior derecha)

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("No hay un objetivo asignado para que la cámara siga.");
            return;
        }

        // Posición deseada de la cámara
        Vector3 desiredPosition = target.position + offset;

        // Suavizado del movimiento de la cámara
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Aplicar límites de la cámara si están activados
        if (useBounds)
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(transform.position.y, minBounds.y, maxBounds.y),
                transform.position.z
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja los límites de la cámara en el Editor
        if (useBounds)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(new Vector3(minBounds.x, minBounds.y, 0), new Vector3(maxBounds.x, minBounds.y, 0));
            Gizmos.DrawLine(new Vector3(maxBounds.x, minBounds.y, 0), new Vector3(maxBounds.x, maxBounds.y, 0));
            Gizmos.DrawLine(new Vector3(maxBounds.x, maxBounds.y, 0), new Vector3(minBounds.x, maxBounds.y, 0));
            Gizmos.DrawLine(new Vector3(minBounds.x, maxBounds.y, 0), new Vector3(minBounds.x, minBounds.y, 0));
        }
    }
}