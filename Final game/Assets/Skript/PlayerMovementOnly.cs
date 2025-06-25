using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovementOnly : MonoBehaviour
{
    [Header("Передвижение")]
    [SerializeField] private float speed = 3f;

    [Header("Спрайт для разворота")]
    [SerializeField] private SpriteRenderer spriteToFlip; 

    [Header("Фейд экран")]
    [SerializeField] private GameObject fadeScreenObject; 

    private SpriteRenderer sr;
    private bool isMoving = true;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isMoving) return;

        float horizontal = Input.GetAxis("Horizontal");

        if (horizontal != 0)
        {
            Move(horizontal);
        }
    }

    private void Move(float direction)
    {
        if (sr != null) sr.flipX = direction < 0;
        transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("NextLevel"))
        {
            StartCoroutine(LevelTransitionSequence());
        }
    }

    private IEnumerator LevelTransitionSequence()
    {
        
        isMoving = false;

        
        yield return new WaitForSeconds(3f);

        
        if (spriteToFlip != null)
        {
            float elapsed = 0f;
            float duration = 2f;
            bool originalFlip = spriteToFlip.flipX;

            while (elapsed < duration)
            {
                spriteToFlip.flipX = Mathf.Lerp(originalFlip ? 1 : 0, !originalFlip ? 1 : 0, elapsed / duration) > 0.5f;
                elapsed += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            spriteToFlip.flipX = !originalFlip;
        }

        
        Screen_folder.Instance.StartFadeOut(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        });
    }
}
