using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float swimForce = 1.5f;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Limits")]
    public float minY = -7f;
    public float maxY = 2f;

    // ATAQUE
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 🟢 NADO (W)
        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.linearVelocity = Vector2.up * swimForce;
        }

        // 🔴 ATAQUE (CTRL)
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            Attack();
        }

        // 🔥 trava posição
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;

        // 🔥 bloqueia só a direção errada
        if (transform.position.y >= maxY && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }

        if (transform.position.y <= minY && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
    }

    void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        anim.SetTrigger("attack");

        // 🔁 libera ataque depois de um tempo (ajusta conforme animação)
        Invoke(nameof(ResetAttack), 0.5f);
    }

    void ResetAttack()
    {
        isAttacking = false;
    }
}