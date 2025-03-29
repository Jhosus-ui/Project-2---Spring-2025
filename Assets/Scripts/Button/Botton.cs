using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class LoadSceneButton : MonoBehaviour, IPointerEnterHandler
{
    [Header("Configuración de Escena")]
    [Tooltip("Nombre de la escena a cargar")]
    public string sceneName;

    [Header("Configuración de Sonido")]
    [Tooltip("Sonido al pasar el cursor por encima")]
    public AudioClip hoverSound;

    [Tooltip("Sonido al hacer clic")]
    public AudioClip clickSound;

    [Tooltip("Volumen de los sonidos (0-1)")]
    [Range(0, 1)] public float volume = 1f;

    private AudioSource audioSource;

    private void Start()
    {
       
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.volume = volume;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (clickSound != null)
            {
                audioSource.PlayOneShot(clickSound);
            }
            float delay = clickSound != null ? clickSound.length * 0.5f : 0;
            Invoke("LoadTargetScene", delay);
        });
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null && GetComponent<Button>().interactable)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    private void LoadTargetScene()
    {
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1f;
    }
}