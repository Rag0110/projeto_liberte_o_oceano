using UnityEngine;

public class BreakableObstacle : MonoBehaviour
{
    [Header("Stats")]
    public int health = 1;

    private bool isBroken = false;

    // Getter para checar se já quebrou
    public bool IsBroken()
    {
        return isBroken;
    }

    // Recebe dano do Player/Hitbox
    public void TakeDamage(int damage)
    {
        if (isBroken) return;

        Debug.Log("TAKE DAMAGE CHAMADO");

        health -= damage;

        if (health <= 0)
        {
            Break();
        }
    }

    // Quebra o obstáculo
    public void Break()
    {
        if (isBroken) return;

        isBroken = true;
        Debug.Log("QUEBROU");
        Destroy(gameObject);
    }

    // Mantém trigger limpo: não chama dano no player aqui
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isBroken) return;

        // Se a colisão for com a hitbox de ataque, ignore (o TakeDamage já é chamado lá)
        AttackHitbox attack = collision.GetComponent<AttackHitbox>();
        if (attack != null)
            return;

        // Não aplica dano direto ao player aqui
        // PlayerController vai cuidar disso
    }
}