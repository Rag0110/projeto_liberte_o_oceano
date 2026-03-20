using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HeartUI : MonoBehaviour
{
    private Image img;
    private Vector3 originalScale;

    void Awake()
    {
        img = GetComponent<Image>();
        originalScale = transform.localScale;
    }

    public void SetSprite(Sprite sprite)
    {
        img.sprite = sprite;
    }

    public void PlayDamageEffect()
    {
        Debug.Log("POP!");
        StopAllCoroutines(); // evita bug de spam
        StartCoroutine(PopEffect());
    }

    IEnumerator PopEffect()
    {
        // cresce rápido
        transform.localScale = originalScale * 1.5f;

        yield return new WaitForSeconds(0.1f);

        // volta suave
        transform.localScale = originalScale;
    }
}