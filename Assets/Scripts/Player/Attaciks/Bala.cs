using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bala : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 10f;
    public float distanciaMaxima = 15f;
    public int dano = 1; // Renombrado de 'damage' a 'dano' para evitar conflictos

    // Dirección se calcula al iniciar, no es pública
    private Vector2 direccionMovimiento;
    private Vector2 posicionInicial;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        posicionInicial = transform.position;
        rb.linearVelocity = direccionMovimiento * velocidad;
    }

    // Método para inicializar desde PlayerAttack
    public void Configurar(Vector2 dir, float vel, float distMax, int danoBala)
    {
        direccionMovimiento = dir.normalized;
        velocidad = vel;
        distanciaMaxima = distMax;
        dano = danoBala;
    }

    void Update()
    {
        if (Vector2.Distance(posicionInicial, transform.position) >= distanciaMaxima)
        {
            DestruirBala();
        }
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Enemy")) // Asegúrate que tus enemigos tengan este tag
        {
            EnemyHealth saludEnemigo = otro.GetComponent<EnemyHealth>();
            if (saludEnemigo != null) saludEnemigo.TakeDamage(dano);

            DestruirBala();
        }
        else if (otro.CompareTag("Mapa")) // Ejemplo de otro objeto
        {
            DestruirBala();
        }
    }

    void DestruirBala()
    {
        Destroy(gameObject);
    }
}