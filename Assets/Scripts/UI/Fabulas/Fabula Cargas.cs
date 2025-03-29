using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FabulasAmmo : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxCharges = 3; // M�ximo de cargas disponibles
    public float rechargeTime = 2f; // Tiempo para recargar una carga
    public Image[] chargeImages; // Im�genes UI que representan las cargas (debe tener tama�o igual a maxCharges)

    [Header("Visual Feedback")]
    public Color activeChargeColor = Color.white;
    public Color emptyChargeColor = Color.gray;

    [Header("Sound Effects")]
    public AudioClip rechargeSound;
    private AudioSource audioSource;

    private int currentCharges;
    private Coroutine rechargeCoroutine;
    private PlayerAttack playerAttack;

    private void Awake()
    {
        playerAttack = GetComponent<PlayerAttack>();
        audioSource = GetComponent<AudioSource>();
        InitializeCharges();
    }

    private void InitializeCharges()
    {
        currentCharges = maxCharges;
        UpdateChargeUI();

        // Asegurarse de que hay suficientes im�genes
        if (chargeImages.Length != maxCharges)
        {
            Debug.LogWarning("El n�mero de im�genes de carga no coincide con maxCharges");
        }
    }

    private void UpdateChargeUI()
    {
        for (int i = 0; i < chargeImages.Length; i++)
        {
            if (chargeImages[i] != null)
            {
                chargeImages[i].color = i < currentCharges ? activeChargeColor : emptyChargeColor;
            }
        }
    }

    public bool CanShoot()
    {
        return currentCharges > 0;
    }

    public void ConsumeCharge()
    {
        if (currentCharges <= 0) return;

        currentCharges--;
        UpdateChargeUI();

        if (rechargeCoroutine != null)
        {
            StopCoroutine(rechargeCoroutine);
        }
        rechargeCoroutine = StartCoroutine(Recharge());
    }

    private IEnumerator Recharge()
    {
        yield return new WaitForSeconds(rechargeTime);

        if (currentCharges < maxCharges)
        {
            currentCharges++;
            UpdateChargeUI();

            // Reproducir sonido de recarga
            if (rechargeSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(rechargeSound);
            }

            if (currentCharges < maxCharges)
            {
                rechargeCoroutine = StartCoroutine(Recharge());
            }
        }
    }

    // M�todo para resetear cargas (opcional)
    public void ResetCharges()
    {
        currentCharges = maxCharges;
        UpdateChargeUI();
    }
}
