using System.Collections;
using UnityEngine;

public class Screen_folder : MonoBehaviour
{
    public static Screen_folder Instance { get; private set; }

    [SerializeField] private float fadeSpeed = 1f;

    private UnityEngine.UI.Image darkImage;
    private bool isFading = false;

    void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        darkImage = GetComponent<UnityEngine.UI.Image>();
        

      
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        
        gameObject.SetActive(true);
        StartCoroutine(FadeIn());
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
   
        if (this != null && gameObject != null)
        {
            gameObject.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }


    public void StartFadeOut(System.Action onFadeComplete)
    {
        if (isFading) return;

     
        gameObject.SetActive(true);
        StartCoroutine(FadeOut(onFadeComplete));
    }
    private IEnumerator FadeOut(System.Action onFadeComplete)
    {
        isFading = true;

        Color color = darkImage.color;
        color.a = 0f;
        darkImage.color = color;

        while (color.a < 1f)
        {
            color.a += fadeSpeed * Time.deltaTime;
            color.a = Mathf.Clamp01(color.a);
            darkImage.color = color;
            yield return null;
        }

        onFadeComplete?.Invoke();
        isFading = false;
    }
    public IEnumerator FadeIn()
    {
        isFading = true;
        Color color = darkImage.color;
        color.a = 1f;
        darkImage.color = color;

        while (color.a > 0f)
        {
            color.a -= fadeSpeed * Time.deltaTime;
            color.a = Mathf.Clamp01(color.a);
            darkImage.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
        isFading = false;
    }
}