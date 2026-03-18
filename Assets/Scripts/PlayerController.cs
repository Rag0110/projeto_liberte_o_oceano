using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float swimForce = 1.5f;
    private Rigidbody2D rb;

    [Header("Limits")]
    public float minY = -7f;
    public float maxY = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.linearVelocity = Vector2.up * swimForce;
        }

        // 🔥 trava posição
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;

        // 🔥 bloqueia só a direção errada (sem travar o player)
        if (transform.position.y >= maxY && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }

        if (transform.position.y <= minY && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        }
    }
}