using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float speed = 1f;

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.position.x < -46f)
        {
            transform.position += new Vector3(92f, 0, 0);
        }
    }
}
