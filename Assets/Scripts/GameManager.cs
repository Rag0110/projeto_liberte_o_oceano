using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameMode { Flappy, Transitioning, Exploration, Returning }
    public GameMode currentMode = GameMode.Flappy;

    public int scoreToSwitch = 100;
    private int nextSwitchScore;

    [Header("Fish Net")]
    public FishNetSpawner fishNetSpawner;

    [Header("Return Settings")]
    public float returnMoveTime = 1f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        nextSwitchScore = scoreToSwitch;
    }

    public void CheckModeSwitch(int currentScore)
    {
        if (currentMode == GameMode.Flappy && currentScore >= nextSwitchScore)
        {
            StartTransition();
        }
    }

    void StartTransition()
    {
        currentMode = GameMode.Transitioning;

        foreach (var s in FindObjectsByType<ObstacleSpawner>(FindObjectsSortMode.None))
            s.StopSpawning();

        // Para o auto score na transição
        if (ScoreManager.instance != null)
            ScoreManager.instance.StopAutoScore();

        Debug.Log("Transição iniciada...");

        StartCoroutine(WaitForObstaclesClear());
    }

    IEnumerator WaitForObstaclesClear()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
            ObstacleMover[] movers = FindObjectsByType<ObstacleMover>(FindObjectsSortMode.None);

            if (obstacles.Length == 0 && movers.Length == 0)
            {
                EnterExploration();
                yield break;
            }
        }
    }

    void EnterExploration()
    {
        currentMode = GameMode.Exploration;

        foreach (var p in FindObjectsByType<Parallax>(FindObjectsSortMode.None))
            p.Stop();

        if (fishNetSpawner != null)
            fishNetSpawner.SpawnNet();

        StartCoroutine(WaitForNetsClear());

        Debug.Log("Modo Exploração ativado!");
    }

    IEnumerator WaitForNetsClear()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            FishNet[] nets = FindObjectsByType<FishNet>(FindObjectsSortMode.None);

            if (nets.Length == 0)
            {
                StartReturn();
                yield break;
            }
        }
    }

    void StartReturn()
    {
        currentMode = GameMode.Returning;
        Debug.Log("Voltando pro modo Flappy...");

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.ReturnToStart(returnMoveTime, OnReturnComplete);
        }
    }

    void OnReturnComplete()
    {
        currentMode = GameMode.Flappy;

        foreach (var p in FindObjectsByType<Parallax>(FindObjectsSortMode.None))
            p.Resume();

        foreach (var s in FindObjectsByType<ObstacleSpawner>(FindObjectsSortMode.None))
            s.ResumeSpawning();

        // Reativa o auto score
        if (ScoreManager.instance != null)
            ScoreManager.instance.StartAutoScore();

        nextSwitchScore += scoreToSwitch;

        Debug.Log($"Modo Flappy reativado! Próxima troca em {nextSwitchScore} pontos.");
    }
}