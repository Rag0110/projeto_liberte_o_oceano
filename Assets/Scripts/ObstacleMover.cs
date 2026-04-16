using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    [HideInInspector]
    public float speed = 1f;

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.position.x < -15)
        {
            Destroy(gameObject);
        }
    }
}