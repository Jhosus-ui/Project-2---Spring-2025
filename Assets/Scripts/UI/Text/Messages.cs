using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Configuraci�n de Di�logo")]
    [Tooltip("El objeto de texto TextMeshPro UI que se mostrar�")]
    public TMP_Text dialogueText;

    [Tooltip("Duraci�n en segundos que el texto estar� visible")]
    public float displayDuration = 3f;

    [Tooltip("Velocidad del efecto de aparici�n/desaparici�n")]
    public float fadeSpeed = 2f;

    [Header("Efecto de Desvanecimiento")]
    [Tooltip("Ancho m�ximo del efecto de desvanecimiento horizontal")]
    public float fadeWidth = 20f;

    private bool hasBeenTriggered = false;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        // Asegurarse que el texto est� oculto al inicio
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

        // Efecto de aparici�n
        float timer = 0;
        while (timer < 1f)
        {
            timer += Time.deltaTime * fadeSpeed;
            dialogueText.maxVisibleCharacters = (int)Mathf.Lerp(0, dialogueText.textInfo.characterCount, timer);
            yield return null;
        }
        dialogueText.maxVisibleCharacters = dialogueText.textInfo.characterCount;

        // Esperar el tiempo de visualizaci�n
        yield return new WaitForSeconds(displayDuration);

        // Efecto de desaparici�n
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