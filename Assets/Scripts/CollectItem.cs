using UnityEngine;
using System.Collections;

public class CollectItem : MonoBehaviour
{
    public int points = 10;

    [Header("Feedback")]
    public float popScale = 1.5f;
    public float floatUpDistance = 1f;
    public float feedbackTime = 0.4f;

    private bool collected = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;
            ScoreManager.instance.AddScore(points);

            // Desliga collider pra não coletar de novo
            GetComponent<Collider2D>().enabled = false;

            // Para o movimento se tiver ObstacleMover
            ObstacleMover mover = GetComponent<ObstacleMover>();
            if (mover != null) mover.enabled = false;

            StartCoroutine(CollectFeedback());
        }
    }

    IEnumerator CollectFeedback()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Vector3 originalScale = transform.localScale;
        Vector3 startPos = transform.position;
        float timer = 0f;

        while (timer < feedbackTime)
        {
            timer += Time.deltaTime;
            float t = timer / feedbackTime;

            // Pop: cresce rápido e volta
            float scale = Mathf.Lerp(popScale, 0f, t);
            transform.localScale = originalScale * Mathf.Max(scale, 0f);

            // Flutua pra cima
            transform.position = startPos + Vector3.up * (floatUpDistance * t);

            // Fade out
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 1f - t;
                sr.color = c;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}