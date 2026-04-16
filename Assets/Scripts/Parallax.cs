using UnityEngine;
using UnityEngine.Tilemaps;

public class Parallax : MonoBehaviour
{
    public float speed = 1f;
    public Parallax partner; // a outra cópia da mesma camada

    private float tileLength;
    private bool isMoving = true;

    void Start()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        if (tilemap == null)
            tilemap = GetComponentInChildren<Tilemap>();

        tilemap.CompressBounds();
        tileLength = tilemap.localBounds.size.x;
    }

    void Update()
    {
        if (!isMoving) return;

        transform.Translate(Vector3.left * speed * Time.deltaTime);

        float camLeft = Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect);
        float rightEdge = transform.position.x + tileLength;

        if (rightEdge < camLeft)
        {
            // Cola atrás do parceiro
            float partnerRight = partner.transform.position.x + tileLength;
            transform.position = new Vector3(partnerRight, transform.position.y, transform.position.z);
        }
    }

    public void Stop()
    {
        isMoving = false;
    }

    public void Resume()
    {
        isMoving = true;
    }
}