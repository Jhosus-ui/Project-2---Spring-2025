using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UITimedElement : MonoBehaviour
{
    // Code based on previous projects
    [Tooltip("Segundos antes de aparecer")] public float delay = 1f;
    [Tooltip("Segundos visibles")] public float showTime = 2f;
    [Tooltip("Duración del fade")] public float fadeTime = 0.3f;

    private void Start() => StartCoroutine(ShowHideRoutine());

    private IEnumerator ShowHideRoutine()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.blocksRaycasts = false;

        // Esperar delay inicial
        yield return new WaitForSeconds(delay);

        // Fade in
        float timer = 0;
        while (timer < fadeTime)
        {
            cg.alpha = Mathf.Lerp(0, 1, timer / fadeTime);
            timer += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 1;
        cg.blocksRaycasts = true;

        // Esperar tiempo visible
        yield return new WaitForSeconds(showTime);

        // Fade out
        timer = 0;
        cg.blocksRaycasts = false;
        while (timer < fadeTime)
        {
            cg.alpha = Mathf.Lerp(1, 0, timer / fadeTime);
            timer += Time.deltaTime;
            yield return null;
        }
        cg.alpha = 0;
    }
}