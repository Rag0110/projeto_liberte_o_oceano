using UnityEngine;

public class BreakableObstacle : MonoBehaviour
{
    public int health = 1;

    private bool isDestroyed = false;

    public void TakeDamage(int damage)
    {
        if (isDestroyed) return;

        health -= damage;

        if (health <= 0)
        {
            Break();
        }
    }

    void Break()
    {
        isDestroyed = true;

        // Aqui pode colocar animação depois
        Destroy(gameObject, 0.5f); // tempo em segundos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDestroyed) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Dano no player
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                player.TakeDamage(1, transform.position);
            }

            // Destrói o obstáculo mesmo sem ataque
            Break();
        }
    }
}