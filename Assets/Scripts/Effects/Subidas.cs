using UnityEngine;

public class PlataformaElevadoraSimple : MonoBehaviour
{
    [Header("Movimiento")]
    public float distanciaSubida = 3f;
    public float velocidad = 2f;
    public float delayBajada = 2f;

    [Header("Activación")]
    public GameObject enemigoRequerido;

    private Vector3 posInicial;
    private Vector3 posFinal;
    private float tiempoFuera;
    private bool playerPresente;
    private bool activa;

    private void Start()
    {
        posInicial = transform.position;
        posFinal = posInicial + Vector3.up * distanciaSubida;
        activa = enemigoRequerido == null;
    }

    private void Update()
    {
        if (!activa && enemigoRequerido == null) activa = true;
        if (!activa) return;

        Vector3 targetPos = playerPresente ? posFinal : posInicial;
        float speed = playerPresente ? velocidad : velocidad * 0.5f;

        if (!playerPresente) tiempoFuera += Time.deltaTime;

        if (playerPresente || tiempoFuera >= delayBajada)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerPresente = true;
            tiempoFuera = 0f;
            col.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerPresente = false;
            col.transform.SetParent(null);
        }
    }
}