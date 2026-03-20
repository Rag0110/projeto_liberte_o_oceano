using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
            TakeDamage(1);
        }

        // 🔥 trava posição
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;

        // 🔥 bloqueia só a direção errada
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

    // ❤️ SISTEMA DE VIDA
    public void TakeDamage(int damage)
    {
        if (isInvulnerable) return; // 👈 BLOQUEIA DANO

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        // 💥 anima o coração perdido
        hearts[currentHealth].PlayDamageEffect();

        UpdateHearts();

        // 👻 ativa invulnerabilidade
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
    }

    // 👻 COROUTINE DE INVULNERABILIDADE
    IEnumerator Invulnerability()
    {
        isInvulnerable = true;

        float timer = 0f;

        while (timer < invulnerableTime)
        {
            // transparente
            sr.color = new Color(1f, 1f, 1f, 0.3f);
            yield return new WaitForSeconds(0.1f);

            // normal
            sr.color = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(0.1f);

            timer += 0.2f;
        }

        // garante reset
        sr.color = new Color(1f, 1f, 1f, 1f);
        isInvulnerable = false;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("COLIDIU COM: " + collision.name);

        if (collision.CompareTag("Obstacle"))
        {
            Debug.Log("É OBSTÁCULO!");
            TakeDamage(1);
        }
    }
}