using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float swimForce = 1.5f;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Limits")]
    public float minY = -7f;
    public float maxY = 2f;

    // ATAQUE
    private bool isAttacking = false;

    // ❤️ VIDA
    [Header("Health")]
    public int maxHealth = 4;
    private int currentHealth;

    [Header("UI Hearts")]
    public HeartUI[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    // 👻 INVULNERABILIDADE
    private bool isInvulnerable = false;
    public float invulnerableTime = 1.5f;
    private SpriteRenderer sr;

    // 💥 KNOCKBACK
    [Header("Knockback")]
    public float knockbackForceX = 5f;
    public float knockbackForceY = 2f;

    // 🎮 GAME OVER
    [Header("Game Over UI")]
    public GameObject gameOverText;
    private bool isGameOver = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
        UpdateHearts();
    }

    void Update()
    {
        if (isGameOver)
        {
            if (Input.anyKeyDown)
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            return;
        }

        // 🟢 NADO (W)
        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.linearVelocity = Vector2.up * swimForce;
        }

        // 🔴 ATAQUE (CTRL)
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Attack();
        }

        // 🧪 TESTE DE DANO (H)
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1, transform.position);
        }

        // 🔥 trava posição
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;

        // 🔥 bloqueia direção errada
        if (transform.position.y >= maxY && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }

        if (transform.position.y <= minY && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
    }

    void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        anim.SetTrigger("attack");

        Invoke(nameof(ResetAttack), 0.5f);
    }

    void ResetAttack()
    {
        isAttacking = false;
    }

    // ❤️ VIDA + KNOCKBACK
    public void TakeDamage(int damage, Vector3 obstaclePosition)
    {
        if (isInvulnerable || isGameOver) return;

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        // 💥 anima coração (corrigido)
        if (currentHealth > 0)
        {
            hearts[currentHealth - 1].PlayDamageEffect();
        }

        UpdateHearts();

        // 💥 knockback corrigido
        ApplyKnockback(obstaclePosition);

        // 👻 invulnerabilidade
        StartCoroutine(Invulnerability());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
                hearts[i].SetSprite(fullHeart);
            else
                hearts[i].SetSprite(emptyHeart);
        }
    }

    void Die()
    {
        Debug.Log("Morreu");

        gameOverText.SetActive(true);
        Time.timeScale = 0f;

        isGameOver = true;
    }

    // 👻 INVULNERABILIDADE
    System.Collections.IEnumerator Invulnerability()
    {
        isInvulnerable = true;

        float timer = 0f;

        while (timer < invulnerableTime)
        {
            sr.color = new Color(1f, 1f, 1f, 0.3f);
            yield return new WaitForSeconds(0.1f);

            sr.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.1f);

            timer += 0.2f;
        }

        sr.color = Color.white;
        isInvulnerable = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            TakeDamage(1, collision.transform.position);
        }
    }

    // 💥 KNOCKBACK CORRIGIDO
    void ApplyKnockback(Vector3 obstaclePosition)
    {
        float directionY = transform.position.y > obstaclePosition.y ? 1f : -1f;

        rb.linearVelocity = new Vector2(-knockbackForceX, directionY * knockbackForceY);
    }
}