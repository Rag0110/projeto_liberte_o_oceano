using UnityEngine;
using System.Collections;

public class FishNet : MonoBehaviour
{
    public int health = 3;
    private bool isBroken = false;
    private Animator anim;
    private SpriteRenderer sr;

    [Header("Movement")]
    public float riseSpeed = 1.5f;
    public float maxY = 2f; // mesmo limite Y do player

    private bool isRising = true;

    [Header("Fade")]
    public float fadeTime = 1f;

    [Header("Hit Feedback")]
    public float shakeDuration = 0.2f;
    public float shakeIntensity = 0.1f;
    public float flashDuration = 0.15f;
    public Color hitColor = Color.red;

    [Header("Score")]
    public int scoreValue = 50;

    private Vector3 originalPosition;
    private bool isFading = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        originalPosition = transform.localPosition;

        if (anim != null)
        {
            anim.ResetTrigger("Break");
            anim.Play("Idle", 0, 0f);
        }
    }

    void Update()
    {
        if (!isRising || isBroken || isFading) return;

        // Sobe devagar
        transform.Translate(Vector3.up * riseSpeed * Time.deltaTime);

        // Chegou no limite da água
        if (transform.position.y >= maxY)
        {
            isRising = false;
            StartCoroutine(EscapeFadeOut());
        }
    }

    public void TakeDamage(int damage)
    {
        if (isBroken || isFading) return;

        health -= damage;

        if (health <= 0)
        {
            Break();
        }
        else
        {
            StartCoroutine(HitFlash());
            StartCoroutine(Shake());
        }
    }

    IEnumerator HitFlash()
    {
        sr.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = Color.white;
    }

    IEnumerator Shake()
    {
        float timer = 0f;
        Vector3 shakeOrigin = transform.localPosition;

        while (timer < shakeDuration)
        {
            float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
            float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
            transform.localPosition = shakeOrigin + new Vector3(offsetX, offsetY, 0f);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = shakeOrigin;
    }

    void Break()
    {
        if (isBroken) return;
        isBroken = true;
        isRising = false;

        StopAllCoroutines();
        sr.color = Color.white;

        if (anim != null)
            anim.SetTrigger("Break");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        // Dá pontos ao destruir
        if (ScoreManager.instance != null)
            ScoreManager.instance.AddScore(scoreValue);

        StartCoroutine(FadeOut());
    }

    // Fade quando a rede escapa (sem pontos)
    IEnumerator EscapeFadeOut()
    {
        isFading = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        yield return StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float timer = 0f;
        Color original = sr.color;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);
            sr.color = new Color(original.r, original.g, original.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }
}