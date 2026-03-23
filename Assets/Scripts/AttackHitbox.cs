using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Acertou algo: " + collision.name);

        BreakableObstacle obstacle = collision.GetComponent<BreakableObstacle>();

        if (obstacle != null)
        {
            Debug.Log("ACERTOU OBSTÁCULO!");
            obstacle.TakeDamage(1);
        }
    }
}