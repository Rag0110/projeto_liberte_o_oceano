using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;

    public float spawnX = 10f;
    public float minY = -2f;
    public float maxY = 2f;

    public float spawnInterval = 4f;

    void Start()
    {
        InvokeRepeating("SpawnItem", 2f, spawnInterval);
    }

    void SpawnItem()
    {
        float randomY = Random.Range(minY, maxY);

        Vector3 spawnPos = new Vector3(spawnX, randomY, 0);

        Instantiate(itemPrefab, spawnPos, Quaternion.identity);
    }
}
