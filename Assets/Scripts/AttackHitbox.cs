using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Breakable"))
        {
            BreakableObstacle breakable = collision.GetComponent<BreakableObstacle>();

            if (breakable != null)
            {
                breakable.TakeDamage(damage);
            }
        }
    }
}