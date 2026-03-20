using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int score = 0;

    public TextMeshProUGUI scoreText;

    Vector3 originalScale;

    public float autoScoreTime = 1f; // tempo para ganhar 1 ponto

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        originalScale = scoreText.transform.localScale;
        UpdateScore();

        StartCoroutine(AutoScore());
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScore();
        StartCoroutine(PopEffect());
    }

    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
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