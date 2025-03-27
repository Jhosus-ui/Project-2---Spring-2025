using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Configuración de Diálogo")]
    [Tooltip("El objeto de texto TextMeshPro UI que se mostrará")]
    public TMP_Text dialogueText;

    [Tooltip("Duración en segundos que el texto estará visible")]
    public float displayDuration = 3f;

    [Tooltip("Velocidad del efecto de aparición/desaparición")]
    public float fadeSpeed = 2f;

    [Header("Efecto de Desvanecimiento")]
    [Tooltip("Ancho máximo del efecto de desvanecimiento horizontal")]
    public float fadeWidth = 20f;

    private bool hasBeenTriggered = false;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        // Asegurarse que el texto está oculto al inicio
        if (dialogueText != null)
        {
            dialogueText.gameObject.SetActive(false);
            dialogueText.maxVisibleCharacters = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenTriggered && dialogueText != null)
        {
            hasBeenTriggered = true;
            StartCoroutine(ShowDialogue());
        }
    }

    private IEnumerator ShowDialogue()
    {
        // Activar el texto
        dialogueText.gameObject.SetActive(true);

        // Efecto de aparición
        float timer = 0;
        while (timer < 1f)
        {
            timer += Time.deltaTime * fadeSpeed;
            dialogueText.maxVisibleCharacters = (int)Mathf.Lerp(0, dialogueText.textInfo.characterCount, timer);
            yield return null;
        }
        dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;

        // Esperar el tiempo de visualización
        yield return new WaitForSeconds(displayDuration);

        // Efecto de desaparición
        timer = 0;
        while (timer < 1f)
        {
            timer += Time.deltaTime * fadeSpeed;
            dialogueText.maxVisibleCharacters = (int)Mathf.Lerp(dialogueText.textInfo.characterCount, 0, timer);
            yield return null;
        }
        dialogueText.maxVisibleCharacters = 0;

        // Desactivar el texto
        dialogueText.gameObject.SetActive(false);
    }
}