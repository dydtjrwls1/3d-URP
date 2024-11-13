using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : SingleTon<ScreenFader>
{
    public enum ScreenType
    {
        Loading,
        GameOver
    }

    [SerializeField]
    CanvasGroup loadingCanvas;

    [SerializeField]
    CanvasGroup gameOverCanvas;

    [SerializeField]
    float fadeDuration = 0.5f;

    public IEnumerator FadeIn()
    {
        CanvasGroup canvas;

        if(loadingCanvas.alpha > 0.1f)
        {
            canvas = loadingCanvas;
        }
        else
        {
            canvas = gameOverCanvas;
        }

        yield return StartCoroutine(Fade(0.0f, canvas));

        canvas.gameObject.SetActive(false);
    }

    public IEnumerator FadeOut(ScreenType type)
    {
        CanvasGroup canvas;

        if (type == ScreenType.Loading)
        {
            canvas = loadingCanvas;
        }
        else
        {
            canvas = gameOverCanvas;
        }

        canvas.gameObject.SetActive(true);  

        yield return StartCoroutine(Fade(1.0f, canvas));
    }

    IEnumerator Fade(float finalAlpha, CanvasGroup canvas)
    {
        float fadeSpeed = Mathf.Abs(canvas.alpha - finalAlpha) / fadeDuration;

        while(!Mathf.Approximately(canvas.alpha, finalAlpha))
        {
            canvas.alpha = Mathf.MoveTowards(canvas.alpha, finalAlpha, fadeSpeed * Time.deltaTime);

            yield return null;
        }

        canvas.alpha = finalAlpha;
    }

}
