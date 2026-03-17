using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject itemPrefab;
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;

    public void TrySpawnItem(float x, float y)
    {
        if (itemPrefab == null) return;

        if (Random.value < spawnChance)
        {
            Instantiate(itemPrefab, new Vector3(x, y, 0f), Quaternion.identity);
        }
    }
}