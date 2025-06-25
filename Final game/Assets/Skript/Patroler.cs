using UnityEngine;

public class Patroler : MonoBehaviour
{
    [Header("Параметры патруля")]
    [SerializeField] private Transform patrolPoint;
    [SerializeField] private float speed = 2f;
    [SerializeField] private int positionOnPatrol = 3;

    [Header("Урон игроку")]
    [SerializeField] private int damageToPlayer = 1;

    public int DamageToPlayer => damageToPlayer;

    [Header("Компоненты")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private bool movingRight = false;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (patrolPoint == null) return;

        float moveDir = movingRight ? 1 : -1;

       
        if (transform.position.x > patrolPoint.position.x + positionOnPatrol)
        {
            movingRight = false;
        }
        else if (transform.position.x < patrolPoint.position.x - positionOnPatrol)
        {
            movingRight = true;
        }

        transform.position += new Vector3(moveDir * speed * Time.deltaTime, 0, 0);
        spriteRenderer.flipX = movingRight;
    }
}

