using UnityEngine;

public class CollectableCell : MonoBehaviour
{
    public int puntos = 1;
    public float levitationHeight = 0.5f; // Altura de la levitaci�n
    public float levitationSpeed = 1f; // Velocidad del movimiento

    private Vector3 startPosition;
    private float randomOffset;

    private void Start()
    {
        startPosition = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI); // Offset aleatorio para variedad
    }

    private void Update()
    {
        // Efecto de levitaci�n usando una funci�n senoidal
        float newY = startPosition.y + Mathf.Sin(Time.time * levitationSpeed + randomOffset) * levitationHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        // Rotaci�n opcional (descomenta si quieres que rote)
        // transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreSystem.instance.AgregarPuntos(puntos);
            Destroy(gameObject);
        }
    }
}