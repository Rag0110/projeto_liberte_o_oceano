using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float swimForce = 1.5f;
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D col;

    [Header("Limits")]
    public float minY = -7f;
    public float maxY = 2f;

    private bool isAttacking = false;

    [Header("Health")]
    public int maxHealth = 4;
    private int currentHealth;

    [Header("UI Hearts")]
    public HeartUI[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private bool isInvulnerable = false;
    public float invulnerableTime = 1.5f;
    private SpriteRenderer sr;

    [Header("Knockback")]
    public float knockbackForceX = 5f;
    public float knockbackForceY = 2f;

    [Header("Game Over UI")]
    public GameObject gameOverText;

    private bool isGameOver = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        currentHealth = maxHealth;
        UpdateHearts();

        // garante que começa escondido
        if (gameOverText != null)
        {
            Debug.Log("Desativando o objeto: " + gameOverText.name);
            gameOverText.SetActive(false);
        }
    }

    void Update()
    {
        // 👉 SE MORREU, só espera tecla
        if (isGameOver)
        {
            if (Input.anyKeyDown)
            {
                Time.timeScale = 1f; // 🔥 volta o tempo ao normal
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            return; // 🚨 trava o resto do código
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.linearVelocity = Vector2.up * swimForce;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(1, transform.position);
        }

        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;

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

    public void TakeDamage(int damage, Vector3 obstaclePosition)
    {
        if (isInvulnerable || isGameOver) return;

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        if (currentHealth > 0)
        {
            hearts[currentHealth - 1].PlayDamageEffect();
        }

        UpdateHearts();

        ApplyKnockback(obstaclePosition);

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

        isGameOver = true;

        // 🧊 para o tempo do jogo
        Time.timeScale = 0f;

        // 👻 esconde o player
        sr.enabled = false;

        // ⛔ desativa colisão
        if (col != null)
            col.enabled = false;

        // ⛔ para movimento
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        // mostra o texto
        if (gameOverText != null)
            gameOverText.SetActive(true);
    }

    IEnumerator Invulnerability()
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

        sr.color = new Color(1f, 1f, 1f, 1f);
        isInvulnerable = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            TakeDamage(1, collision.transform.position);
        }
    }

    void ApplyKnockback(Vector3 obstaclePosition)
    {
        float directionY = transform.position.y > obstaclePosition.y ? 1f : -1f;

        rb.linearVelocity = new Vector2(-knockbackForceX, directionY * knockbackForceY);
    }
}