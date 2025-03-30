using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PulseHealingSystem : MonoBehaviour
{
    [Header("Configuración Pulsos")]
    public int maxPulses = 3;
    public float rechargeTime = 8f;
    public int healthPerPulse = 1;
    public Image[] pulseImages;

    [Header("Referencias")]
    public PlayerHealth playerHealth; 

    [Header("Apariencia")]
    public Color activeColor = new Color(0.2f, 0.8f, 1f);
    public Color inactiveColor = new Color(0.3f, 0.3f, 0.3f);

    [Header("Sound Effects")]
    public AudioClip healSound;
    public AudioClip rechargeSound;
    private AudioSource audioSource;

    private int currentPulses;
    private Coroutine rechargeRoutine;
    private bool isHealing = false;

    private void Start()
    {
        // Buscar automáticamente al jugador si no está asignado
        if (playerHealth == null)
        {
            playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
        }

        audioSource = GetComponent<AudioSource>();
        currentPulses = maxPulses;
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerHealth != null)
        {
            TryHeal();
        }
    }

    private void TryHeal()
    {
        // Verificar condiciones para curar
        if (currentPulses <= 0 ||
            playerHealth == null ||
            playerHealth.currentHealth >= playerHealth.maxHealth ||
            isHealing)
        {
            return;
        }

        StartCoroutine(HealProcess());
    }

    private IEnumerator HealProcess()
    {
        isHealing = true;

        currentPulses--;
        UpdateUI();
        playerHealth.Heal(healthPerPulse);
        if (healSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(healSound);
        }

        if (rechargeRoutine == null && currentPulses < maxPulses)
        {
            rechargeRoutine = StartCoroutine(RechargePulse());
        }

        yield return new WaitForSeconds(0.5f);
        isHealing = false;
    }

    private IEnumerator RechargePulse()
    {
        yield return new WaitForSeconds(rechargeTime);

        currentPulses++;
        UpdateUI();

        if (rechargeSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(rechargeSound);
        }

        if (currentPulses < maxPulses)
        {
            rechargeRoutine = StartCoroutine(RechargePulse());
        }
        else
        {
            rechargeRoutine = null;
        }
    }

    private void UpdateUI()
    {
        for (int i = 0; i < pulseImages.Length; i++)
        {
            if (pulseImages[i] != null)
            {
                pulseImages[i].color = i < currentPulses ? activeColor : inactiveColor;

                if (i == currentPulses && rechargeRoutine != null)
                {
                    pulseImages[i].fillAmount = Mathf.PingPong(Time.time * 2, 1);
                }
                else
                {
                    pulseImages[i].fillAmount = 1;
                }
            }
        }
    }

    public void ResetPulses()
    {
        if (rechargeRoutine != null)
        {
            StopCoroutine(rechargeRoutine);
            rechargeRoutine = null;
        }

        currentPulses = maxPulses;
        UpdateUI();
    }
}
