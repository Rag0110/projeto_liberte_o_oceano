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

    private int obstaclesToSpawn;
    private int spawnedInGroup = 0;

    void Start()
    {
        if (!Application.isPlaying) return;

        Invoke(nameof(SpawnRoutine), 1f);
    }

    void SpawnRoutine()
    {
        if (!Application.isPlaying) return;

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
        if (!Application.isPlaying) return;

        if (Camera.main == null)
        {
            Debug.LogWarning("Camera.main não encontrada! ObstacleSpawner cancelado.");
            return;
        }

        // Escolhe tipo de obstáculo
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

        // Calcula posição X fora da tela
        float screenRight = Camera.main.transform.position.x + (Camera.main.orthographicSize * Camera.main.aspect);
        float spawnX = screenRight + extraSpawnOffset;

        // Alturas
        float topHeight = topPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
        float bottomHeight = bottomPrefab.GetComponent<SpriteRenderer>().bounds.size.y;

        // Posições Y
        float topY = maxY - (topHeight / 2f);
        float bottomY = minY + (bottomHeight / 2f);

        // Instancia obstáculos
        Instantiate(topPrefab, new Vector3(spawnX, topY, 0f), Quaternion.identity);
        Instantiate(bottomPrefab, new Vector3(spawnX, bottomY, 0f), Quaternion.identity);

        // Centro do gap
        float bottomInnerEdge = bottomY + (bottomHeight / 2f);
        float topInnerEdge = topY - (topHeight / 2f);
        float gapCenter = (bottomInnerEdge + topInnerEdge) / 2f;

        // 🔥 SISTEMA CORRIGIDO (NUNCA SOBREPOR)
        int spawnRoll = Random.Range(0, 100);

        if (spawnRoll < 50)
        {
            // ITEM
            if (itemSpawner != null)
            {
                itemSpawner.TrySpawnItem(spawnX, gapCenter);
            }
        }
        else if (spawnRoll < 80)
        {
            // BREAKABLE
            if (breakablePrefab != null)
            {
                Instantiate(breakablePrefab, new Vector3(spawnX, gapCenter, 0f), Quaternion.identity);
            }
        }
        // else: não spawna nada
    }
}