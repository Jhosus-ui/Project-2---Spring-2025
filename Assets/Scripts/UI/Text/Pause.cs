using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    [Header("Referencias UI")]
    public Button pauseButton;
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button exitButton;

    [Header("Configuración de Audio")]
    public AudioMixer masterMixer; // Opcional: Para control por mixer
    private float prePauseVolume; // Guardar volumen antes de pausar
    private AudioSource[] allAudioSources;
    private bool isPaused = false;

    private void Start()
    {
        // Configurar listeners de los botones
        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(ResumeGame);
        exitButton.onClick.AddListener(ExitGame);

        // Inicializar estado del menú
        pauseMenuPanel.SetActive(false);
    }

    private void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;

        // Pausar el tiempo del juego
        Time.timeScale = 0f;

        // Mostrar menú de pausa
        pauseMenuPanel.SetActive(true);
        pauseButton.gameObject.SetActive(false);

        // Pausar todos los audios
        PauseAllAudio();
    }

    private void ResumeGame()
    {
        isPaused = false;

        // Reanudar el tiempo del juego
        Time.timeScale = 1f;

        // Ocultar menú de pausa
        pauseMenuPanel.SetActive(false);
        pauseButton.gameObject.SetActive(true);

        // Reanudar todos los audios
        ResumeAllAudio();
    }

    private void ExitGame()
    {
        // Asegurarse de reanudar el tiempo y audio antes de salir
        Time.timeScale = 1f;
        ResumeAllAudio();

        // Cambiar a la escena del menú principal
        SceneManager.LoadScene("MainMenu");
    }

    private void PauseAllAudio()
    {
        // Opción 1: Usando AudioMixer (recomendado si tienes uno configurado)
        if (masterMixer != null)
        {
            masterMixer.GetFloat("MasterVolume", out prePauseVolume);
            masterMixer.SetFloat("MasterVolume", -80f); // Silenciar completamente
        }
        // Opción 2: Pausar todos los AudioSources individualmente
        else
        {
            allAudioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in allAudioSources)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Pause();
                }
            }
        }
    }

    private void ResumeAllAudio()
    {
        // Opción 1: Usando AudioMixer
        if (masterMixer != null)
        {
            masterMixer.SetFloat("MasterVolume", prePauseVolume);
        }
        // Opción 2: Reanudar todos los AudioSources individualmente
        else
        {
            if (allAudioSources != null)
            {
                foreach (AudioSource audioSource in allAudioSources)
                {
                    audioSource.UnPause();
                }
            }
        }
    }

    private void Update()
    {
        // Pausar con la tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
}