using UnityEngine;

public class CollectItem : MonoBehaviour
{
    public int points = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager.instance.AddScore(points);
            Destroy(gameObject);
        }
    }
}