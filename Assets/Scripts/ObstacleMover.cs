using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    [HideInInspector]
    public float speed = 1f;

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        float camLeft = Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect);

        if (transform.position.x < camLeft - 3f)
        {
            Destroy(gameObject);
        }
    }
}