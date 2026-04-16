using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Top Obstacles")]
    public GameObject topSmall;
    public GameObject topMedium;
    public GameObject topLarge;

    [Header("Bottom Obstacles")]
    public GameObject bottomSmall;
    public GameObject bottomMedium;
    public GameObject bottomLarge;

    [Header("Item Spawner")]
    public ItemSpawner itemSpawner;

    [Header("Breakable Obstacle")]
    public GameObject breakablePrefab;

    [Header("Spawn Settings")]
    public float minY = -10.5f;
    public float maxY = 3f;

    [Header("Spawn Offset")]
    public float extraSpawnOffset = 2f;

    [Header("Group Settings")]
    public int minGroup = 1;
    public int maxGroup = 3;

    public float smallGap = 1.2f;
    public float bigGap = 3.5f;

    [Header("Speed Reference")]
    public Parallax baseParallax;

    private int obstaclesToSpawn;
    private int spawnedInGroup = 0;
    private bool isStopped = false;

    void Start()
    {
        if (!Application.isPlaying) return;
        Invoke(nameof(SpawnRoutine), 1f);
    }

    public void StopSpawning()
    {
        isStopped = true;
        CancelInvoke();
    }

    public void ResumeSpawning()
    {
        isStopped = false;
        spawnedInGroup = 0;
        Invoke(nameof(SpawnRoutine), bigGap);
    }

    float GetBaseSpeed()
    {
        return (baseParallax != null) ? baseParallax.speed : 1f;
    }

    void SetSpeed(GameObject obj)
    {
        ObstacleMover mover = obj.GetComponent<ObstacleMover>();
        if (mover != null)
            mover.speed = GetBaseSpeed();
    }

    void SpawnRoutine()
    {
        if (!Application.isPlaying || isStopped) return;

        if (spawnedInGroup == 0)
        {
            int roll = Random.Range(0, 100);
            if (roll < 50) obstaclesToSpawn = 1;
            else if (roll < 80) obstaclesToSpawn = 2;
            else obstaclesToSpawn = 3;
        }

        SpawnObstacle();
        spawnedInGroup++;

        if (spawnedInGroup >= obstaclesToSpawn)
        {
            spawnedInGroup = 0;
            Invoke(nameof(SpawnRoutine), bigGap);
        }
        else
        {
            Invoke(nameof(SpawnRoutine), smallGap);
        }
    }

    void SpawnObstacle()
    {
        if (!Application.isPlaying || isStopped) return;

        if (Camera.main == null)
        {
            Debug.LogWarning("Camera.main não encontrada! ObstacleSpawner cancelado.");
            return;
        }

        int type = Random.Range(0, 3);
        GameObject topPrefab = null;
        GameObject bottomPrefab = null;

        switch (type)
        {
            case 0: topPrefab = topSmall; bottomPrefab = bottomLarge; break;
            case 1: topPrefab = topMedium; bottomPrefab = bottomMedium; break;
            case 2: topPrefab = topLarge; bottomPrefab = bottomSmall; break;
        }

        if (topPrefab == null || bottomPrefab == null)
        {
            Debug.LogError("Prefab de obstáculo não atribuído!");
            return;
        }

        float screenRight = Camera.main.transform.position.x + (Camera.main.orthographicSize * Camera.main.aspect);
        float spawnX = screenRight + extraSpawnOffset;

        float topHeight = topPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
        float bottomHeight = bottomPrefab.GetComponent<SpriteRenderer>().bounds.size.y;

        float topY = maxY - (topHeight / 2f);
        float bottomY = minY + (bottomHeight / 2f);

        GameObject topObj = Instantiate(topPrefab, new Vector3(spawnX, topY, 0f), Quaternion.identity);
        GameObject bottomObj = Instantiate(bottomPrefab, new Vector3(spawnX, bottomY, 0f), Quaternion.identity);

        SetSpeed(topObj);
        SetSpeed(bottomObj);

        float bottomInnerEdge = bottomY + (bottomHeight / 2f);
        float topInnerEdge = topY - (topHeight / 2f);
        float gapCenter = (bottomInnerEdge + topInnerEdge) / 2f;

        int spawnRoll = Random.Range(0, 100);

        if (spawnRoll < 50)
        {
            if (itemSpawner != null)
                itemSpawner.TrySpawnItem(spawnX, gapCenter);
        }
        else if (spawnRoll < 80)
        {
            if (breakablePrefab != null)
            {
                GameObject breakObj = Instantiate(breakablePrefab, new Vector3(spawnX, gapCenter, 0f), Quaternion.identity);
                SetSpeed(breakObj);
            }
        }
    }
}