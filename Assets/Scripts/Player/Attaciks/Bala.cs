using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bala : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 10f;
    public float maxDistance = 15f;
    public Vector2 direction = Vector2.right;

    private Vector2 initialPosition;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;

        // Configurar velocidad inicial
        rb.linearVelocity = transform.right * speed;

        // Mensaje de creación
        Debug.Log("Bala creada");
    }

    void Update()
    {
        // Verificar distancia máxima
        if (Vector2.Distance(initialPosition, transform.position) >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Destruir al chocar con enemigos o paredes
        if (collision.CompareTag("Enemy") || collision.CompareTag("Mapa"))
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // Mensaje de destrucción
        Debug.Log("Bala destruida");
    }
}
