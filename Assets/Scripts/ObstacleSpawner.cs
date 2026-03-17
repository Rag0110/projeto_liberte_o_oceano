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

    [Header("Spawn Settings")]
    public float spawnTime = 2f;
    public float spawnX = 10f;

    public float minY = -5f;
    public float maxY = 5f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnObstacle), 1f, spawnTime);
    }

    void SpawnObstacle()
    {
        int type = Random.Range(0, 3);

        GameObject topPrefab = null;
        GameObject bottomPrefab = null;

        switch (type)
        {
            case 0:
                topPrefab = topSmall;
                bottomPrefab = bottomLarge;
                break;

            case 1:
                topPrefab = topMedium;
                bottomPrefab = bottomMedium;
                break;

            case 2:
                topPrefab = topLarge;
                bottomPrefab = bottomSmall;
                break;
        }

        float topHeight = topPrefab.GetComponent<SpriteRenderer>().bounds.size.y;
        float bottomHeight = bottomPrefab.GetComponent<SpriteRenderer>().bounds.size.y;

        float topY = maxY - (topHeight / 2f);
        float bottomY = minY + (bottomHeight / 2f);

        Instantiate(topPrefab, new Vector3(spawnX, topY, 0f), Quaternion.identity);
        Instantiate(bottomPrefab, new Vector3(spawnX, bottomY, 0f), Quaternion.identity);

        // calcula centro do gap
        float bottomInnerEdge = bottomY + (bottomHeight / 2f);
        float topInnerEdge = topY - (topHeight / 2f);
        float gapCenter = (bottomInnerEdge + topInnerEdge) / 2f;

        // chama o ItemSpawner
        if (itemSpawner != null)
        {
            itemSpawner.TrySpawnItem(spawnX, gapCenter);
        }
    }
}