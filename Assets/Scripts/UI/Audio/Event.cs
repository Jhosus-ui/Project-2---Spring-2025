using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OneTimeSoundTrigger : MonoBehaviour
{
    [Header("Configuraci�n de Sonido")]
    [Tooltip("Arrastra aqu� el clip de audio a reproducir")]
    public AudioClip soundToPlay;

    private AudioSource audioSource;
    private bool hasTriggered = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false; // Asegurar que no se repita
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo reproducir si es el jugador, hay sonido y no se ha activado antes
        if (other.CompareTag("Player") && !hasTriggered && soundToPlay != null)
        {
            audioSource.PlayOneShot(soundToPlay); 
            hasTriggered = true; 
            GetComponent<Collider2D>().enabled = false;
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().enabled = true;
        }
    }
}