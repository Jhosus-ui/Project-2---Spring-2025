using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{

    // Option 1: Using AudioMixer
    // Option 2: Resume all AudioSources individually

    [Header("UI References")]
    public Button pauseButton;
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button exitButton;

    [Header("Audio Settings")]
    public AudioMixer masterMixer; 

    [Header("Scene Settings")]
    public string targetSceneName = "MainMenu"; 

    private float prePauseVolume;
    private AudioSource[] allAudioSources;
    private bool isPaused = false;

    private void Start()
    {

        pauseButton.onClick.AddListener(TogglePause);
        resumeButton.onClick.AddListener(ResumeGame);
        exitButton.onClick.AddListener(LoadTargetScene);
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
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
        pauseButton.gameObject.SetActive(false);
        PauseAllAudio();
    }

    private void ResumeGame()
    {
        isPaused = false;

        
        Time.timeScale = 1f;

        
        pauseMenuPanel.SetActive(false);
        pauseButton.gameObject.SetActive(true);

        
        ResumeAllAudio();
    }

    private void LoadTargetScene()
    {
       
        Time.timeScale = 1f;
        ResumeAllAudio();

       
        SceneManager.LoadScene(targetSceneName);
    }

    private void PauseAllAudio()
    {
      
        if (masterMixer != null)
        {
            masterMixer.GetFloat("MasterVolume", out prePauseVolume);
            masterMixer.SetFloat("MasterVolume", -80f); 
        }
       
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
        
        if (masterMixer != null)
        {
            masterMixer.SetFloat("MasterVolume", prePauseVolume);
        }
  
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
     
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
}