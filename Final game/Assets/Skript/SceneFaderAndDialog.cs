using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFaderAndDialog : MonoBehaviour
{
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private Image fadeImage; 
    [SerializeField] private Text displayText; 

    private bool isFading = false;
    private int clickCount = 0;

    private string[] messages = new string[]
    {
        "Спасибо, что напомнил кто я...",
        "Теперь ты свободен. А я остаюсь один со свими делами."
    };

    void Awake()
    {
        if (fadeImage == null)
        {
            Destroy(this); 
            return;
        }
        if (displayText == null)
        {
            Destroy(this); 
            return;
        }
        displayText.text = messages[0];

        StartCoroutine(FadeIn());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isFading)
        {
            HandleButtonClick();
        }
    }

    private void HandleButtonClick()
    {
        clickCount++;

        if (clickCount == 1)
        {
            StartFadeOut(() =>
            {
                ChangeText();
                StartCoroutine(FadeIn());
            });
        }
        else if (clickCount == 2)
        {
            StartFadeOut(() =>
            {
                
                Destroy(gameObject);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            });
        }
    }

    public void StartFadeOut(Action onFadeComplete)
    {
        if (isFading) return;
        StartCoroutine(FadeOut(onFadeComplete));
    }

    private IEnumerator FadeOut(Action onFadeComplete)
    {
        isFading = true;
        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        while (color.a < 1f)
        {
            color.a += fadeSpeed * Time.deltaTime;
            color.a = Mathf.Clamp01(color.a);
            fadeImage.color = color;
            yield return null;
        }

        fadeImage.color = Color.black; 
        onFadeComplete?.Invoke();
        isFading = false;
    }

    private IEnumerator FadeIn()
    {
        isFading = true;
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;

        while (color.a > 0f)
        {
            color.a -= fadeSpeed * Time.deltaTime;
            color.a = Mathf.Clamp01(color.a);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
        isFading = false;
    }

    private void ChangeText()
    {
        displayText.text = messages[1];
    }
}