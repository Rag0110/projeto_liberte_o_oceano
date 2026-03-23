using UnityEngine;

public class BreakableObstacle : MonoBehaviour
{
    public int health = 1;
    private bool isDestroyed = false;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("Animator não encontrado no BreakableObstacle!");
        }
    }

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
        if (isDestroyed) return;
        isDestroyed = true;

        // Toca a animação de quebrar
        if (anim != null)
        {
            anim.SetTrigger("Break"); // Aqui bate com o nome do seu parâmetro
        }

        // Destrói o objeto depois de 0.5s, tempo da animação
        Destroy(gameObject, 0.5f);
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

            // Destrói o obstáculo
            Break();
        }
    }
}