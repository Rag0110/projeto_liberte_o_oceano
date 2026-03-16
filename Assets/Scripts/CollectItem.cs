using UnityEngine;

public class CollectItem : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            ScoreManager.instance.AddScore(50);
            Destroy(gameObject);
        }
    }
}
