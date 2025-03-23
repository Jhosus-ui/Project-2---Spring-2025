using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target; // Objeto que la c�mara seguir� (normalmente el jugador)
    public float smoothSpeed = 0.125f; // Suavizado del movimiento de la c�mara
    public Vector3 offset; // Desplazamiento de la c�mara respecto al objetivo

    [Header("Camera Bounds")]
    public bool useBounds = false; // Activar l�mites de la c�mara
    public Vector2 minBounds; // L�mite m�nimo (esquina inferior izquierda)
    public Vector2 maxBounds; // L�mite m�ximo (esquina superior derecha)

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("No hay un objetivo asignado para que la c�mara siga.");
            return;
        }

        // Posici�n deseada de la c�mara
        Vector3 desiredPosition = target.position + offset;

        // Suavizado del movimiento de la c�mara
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // Aplicar l�mites de la c�mara si est�n activados
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
        // Dibuja los l�mites de la c�mara en el Editor
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