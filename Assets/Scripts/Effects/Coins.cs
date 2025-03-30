using UnityEngine;

public class CollectableCell : MonoBehaviour
{
    public int puntos = 1;
    public float levitationHeight = 0.5f;
    public float levitationSpeed = 1f; 
    public AudioClip collectSound; 

    private Vector3 startPosition;
    private float randomOffset;
    private AudioSource audioSource;

    private void Start()
    {
        startPosition = transform.position;
        randomOffset = Random.Range(0f, 2f * Mathf.PI); 
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        // Efecto de levitación usando una función senoidal
        float newY = startPosition.y + Mathf.Sin(Time.time * levitationSpeed + randomOffset) * levitationHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Reproducir sonido si existe
            if (collectSound != null)
            {
                audioSource.PlayOneShot(collectSound);
            }

            ScoreSystem.instance.AgregarPuntos(puntos);

            // Desactivar el collider y renderer para que no se vea pero el sonido pueda terminar
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Renderer>().enabled = false;

            // Destruir el objeto después de que termine el sonido
            Destroy(gameObject, collectSound != null ? collectSound.length : 0);
        }
    }
}