using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int score = 0;

    public TextMeshProUGUI scoreText;

    Vector3 originalScale;

    public float autoScoreTime = 1f;

    private Coroutine autoScoreCoroutine;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        originalScale = scoreText.transform.localScale;
        UpdateScore();
        StartAutoScore();
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScore();
        StartCoroutine(PopEffect());

        if (GameManager.Instance != null)
            GameManager.Instance.CheckModeSwitch(score);
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    public void StartAutoScore()
    {
        if (autoScoreCoroutine != null)
            StopCoroutine(autoScoreCoroutine);

        autoScoreCoroutine = StartCoroutine(AutoScore());
    }

    public void StopAutoScore()
    {
        if (autoScoreCoroutine != null)
        {
            StopCoroutine(autoScoreCoroutine);
            autoScoreCoroutine = null;
        }
    }

    IEnumerator AutoScore()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoScoreTime);
            AddScore(10);
        }
    }

    IEnumerator PopEffect()
    {
        scoreText.transform.localScale = originalScale * 1.1f;
        yield return new WaitForSeconds(0.1f);
        scoreText.transform.localScale = originalScale;
    }
}