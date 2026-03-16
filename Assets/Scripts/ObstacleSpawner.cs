using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstacle;
    public GameObject itemPrefab;

    public float spawnTime = 2f;
    public float minY = -3f;
    public float maxY = 3f;

    void Start()
    {
        InvokeRepeating("SpawnObstacle", 1f, spawnTime);
    }

    void SpawnObstacle()
    {
        float y = Random.Range(minY, maxY);

        Vector3 spawnPos = new Vector3(10f, y, 0f);

        GameObject newObstacle = Instantiate(obstacle, spawnPos, Quaternion.identity);

        // chance de criar item perto do obstáculo
        if (Random.value > 0.5f)
        {
            Vector3 itemPos = spawnPos + new Vector3(-1.5f, 0.5f, 0f);

            Instantiate(itemPrefab, itemPos, Quaternion.identity);
        }
    }
}
