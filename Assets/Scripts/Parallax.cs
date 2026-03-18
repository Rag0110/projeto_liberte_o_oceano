using UnityEngine;
using UnityEngine.Tilemaps;

public class ParallaxLoop : MonoBehaviour
{
    public float speed = 4f;

    private float length;
    private Vector3 startPos;
    private Tilemap tilemap;

    void Start()
    {
        startPos = transform.position;

        tilemap = GetComponentInChildren<Tilemap>();
        tilemap.CompressBounds();

        length = tilemap.localBounds.size.x;
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x <= startPos.x - length)
        {
            transform.position += new Vector3(length * 2, 0, 0);
        }
    }
}