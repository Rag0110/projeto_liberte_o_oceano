using UnityEngine;
using UnityEngine.UI;
using System;
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

    [Header("Exploration")]
    public float moveSpeed = 5f;

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

    [Header("Pause UI")]
    public GameObject pausePanel;
    private bool isPaused = false;

    private bool isGameOver = false;
    public GameObject attackHitbox;

    private Vector3 startPosition;
    private bool isReturning = false;

    [Header("Sound Effects")]
    public AudioClip hitSound;

    [Header("Music")]
    public AudioSource musicSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb.freezeRotation = true;

        startPosition = transform.position;

        currentHealth = maxHealth;
        UpdateHearts();

        if (gameOverText != null)
            gameOverText.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(false);
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                if (musicSource != null)
                    musicSource.UnPause();
                Time.timeScale = 1f;
                SceneManager.LoadScene(0);
                return;
            }
            else
            {
                isPaused = true;
                Time.timeScale = 0f;
                if (pausePanel != null)
                    pausePanel.SetActive(true);
                if (musicSource != null)
                    musicSource.Pause();
                return;
            }
        }

        if (isPaused && Input.GetKeyDown(KeyCode.Return))
        {
            isPaused = false;
            Time.timeScale = 1f;
            if (pausePanel != null)
                pausePanel.SetActive(false);
            if (musicSource != null)
                musicSource.UnPause();
            return;
        }

        if (isPaused) return;

        if (isReturning) return;

        bool isExploration = GameManager.Instance != null &&
            GameManager.Instance.currentMode == GameManager.GameMode.Exploration;

        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.linearVelocity = Vector2.up * swimForce;
        }

        if (isExploration)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

            if (horizontal > 0)
            {
                sr.flipX = false;
                if (attackHitbox != null)
                    attackHitbox.transform.localPosition = new Vector3(
                        Mathf.Abs(attackHitbox.transform.localPosition.x),
                        attackHitbox.transform.localPosition.y,
                        attackHitbox.transform.localPosition.z);
            }
            else if (horizontal < 0)
            {
                sr.flipX = true;
                if (attackHitbox != null)
                    attackHitbox.transform.localPosition = new Vector3(
                        -Mathf.Abs(attackHitbox.transform.localPosition.x),
                        attackHitbox.transform.localPosition.y,
                        attackHitbox.transform.localPosition.z);
            }

            float spriteHalfWidth = sr.bounds.extents.x;
            float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
            float leftLimit = Camera.main.transform.position.x - camHalfWidth + spriteHalfWidth;
            float rightLimit = Camera.main.transform.position.x + camHalfWidth - spriteHalfWidth;

            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, leftLimit, rightLimit);
            transform.position = pos;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Attack();
        }

        {
            Vector3 pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
            transform.position = pos;
        }

        if (transform.position.y >= maxY && rb.linearVelocity.y > 0)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

        if (transform.position.y <= minY && rb.linearVelocity.y < 0)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
    }

    public void ReturnToStart(float duration, Action onComplete)
    {
        StartCoroutine(SmoothReturn(duration, onComplete));
    }

    IEnumerator SmoothReturn(float duration, Action onComplete)
    {
        isReturning = true;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        sr.flipX = false;
        if (attackHitbox != null)
            attackHitbox.transform.localPosition = new Vector3(
                Mathf.Abs(attackHitbox.transform.localPosition.x),
                attackHitbox.transform.localPosition.y,
                attackHitbox.transform.localPosition.z);

        Vector3 from = transform.position;
        Vector3 to = startPosition;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, timer / duration);
            transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }

        transform.position = startPosition;

        rb.simulated = true;
        isReturning = false;

        onComplete?.Invoke();
    }

    void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        anim.SetTrigger("attack");

        if (attackHitbox != null)
            attackHitbox.SetActive(true);

        Invoke(nameof(ResetAttack), 0.5f);
    }

    void ResetAttack()
    {
        isAttacking = false;

        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public void TakeDamage(int damage, Vector3 obstaclePosition)
    {
        if (isInvulnerable || isGameOver) return;

        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, transform.position);

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        if (currentHealth > 0)
            hearts[currentHealth - 1].PlayDamageEffect();

        UpdateHearts();
        ApplyKnockback(obstaclePosition);
        StartCoroutine(Invulnerability());

        if (currentHealth <= 0)
            Die();
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
        isGameOver = true;
        Time.timeScale = 0f;
        sr.enabled = false;

        if (col != null)
            col.enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (isAttacking) return;
            TakeDamage(1, collision.transform.position);
        }
    }

    void ApplyKnockback(Vector3 obstaclePosition)
    {
        float directionY = transform.position.y > obstaclePosition.y ? 1f : -1f;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(-knockbackForceX, directionY * knockbackForceY), ForceMode2D.Impulse);
    }
}