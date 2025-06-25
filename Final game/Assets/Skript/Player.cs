using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum States
{
    Animation_Player,
    Animation_Player_Jump
}

public class Player : MonoBehaviour
{
    [Header("Передвижение")]
    [SerializeField] private float speed = 7f;
    public float Speed => speed;

    [SerializeField] private float jumpHeight = 10f;

    [Header("Здоровье")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private Image currentHealthBar;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool grounded = false;
    private int currentHealth;
    private bool isTakingDamage = false;
    private Animator anim;

    private static int savedHealth;
    private static bool hasKey = false;

    public AudioSource jumpAudio, punchAudio, keyAudio;

    private States State
    {
        get { return (States)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            currentHealth = maxHealth;
        }
        else if (savedHealth > 0)
        {
            currentHealth = savedHealth;
        }
        else
        {
            currentHealth = maxHealth;
        }

        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            hasKey = false;
        }

        UpdateHealthBar();

    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");

        if (grounded)
        {
            State = States.Animation_Player;
        }
        else
        {
            State = States.Animation_Player_Jump;
        }

        if (horizontal != 0) Move(horizontal);
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            jumpAudio.Play();
            Jump();
        }
    }

    private void Move(float direction)
    {
        if (sr != null) sr.flipX = direction < 0;
        transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);
    }

    private void Jump()
    {
        grounded = false;
        rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
        State = States.Animation_Player_Jump;
    }

    public void TakeDamage(int damage)
    {
        if (isTakingDamage) return;

        isTakingDamage = true;
        currentHealth -= damage;


        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Invoke(nameof(ResetDamageFlag), 0.5f);
        }

        UpdateHealthBar();

        savedHealth = currentHealth;
    }

    private void ResetDamageFlag()
    {
        isTakingDamage = false;
    }

    private void Die()
    {
        savedHealth = 0;
        SceneManager.LoadScene(0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            punchAudio.Play();
            Patroler enemy = collision.gameObject.GetComponent<Patroler>();
            if (enemy != null)
            {
                TakeDamage(enemy.DamageToPlayer);
            }
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }

        if (collision.gameObject.name == "EndLevel")
        {
            if (hasKey)
            {
                savedHealth = currentHealth;

                if (Screen_folder.Instance != null)
                {
                    Screen_folder.Instance.StartFadeOut(() =>
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    });
                }
                else
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

    public void PickupKey()
    {
        keyAudio.Play();
        hasKey = true;
    }

    private void UpdateHealthBar()
    {
        if (currentHealthBar != null)
        {
            currentHealthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasKey = false;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}