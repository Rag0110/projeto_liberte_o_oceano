using UnityEngine;
using System.Collections;

public class FishNetSpawner : MonoBehaviour
{
    public GameObject fishNetPrefab;

    [Header("Spawn Settings")]
    public float spawnBelowScreen = 2f;
    public int netCount = 3;
    public float timeBetweenNets = 3f;

    public void SpawnNet()
    {
        if (fishNetPrefab == null)
        {
            Debug.LogWarning("FishNet prefab não atribuído!");
            return;
        }

        StartCoroutine(SpawnSequence());
    }

    IEnumerator SpawnSequence()
    {
        for (int i = 0; i < netCount; i++)
        {
            float screenBottom = Camera.main.transform.position.y - Camera.main.orthographicSize;
            float spawnY = screenBottom - spawnBelowScreen;

            float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
            float spawnX = Random.Range(
                Camera.main.transform.position.x - camHalfWidth + 1f,
                Camera.main.transform.position.x + camHalfWidth - 1f);

            Instantiate(fishNetPrefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);
            Debug.Log($"Rede {i + 1}/{netCount} spawnada!");

            if (i < netCount - 1)
                yield return new WaitForSeconds(timeBetweenNets);
        }
    }
}