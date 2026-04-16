using UnityEngine;
using UnityEngine.Tilemaps;

public class ParallaxGrid : MonoBehaviour
{
    public float speed = 1f;
    private bool isMoving = true;

    private Transform[] children;
    private float tileLength;

    void Start()
    {
        children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            children[i] = transform.GetChild(i);
        }

        Tilemap tilemap = children[0].GetComponent<Tilemap>();
        tilemap.CompressBounds();
        tileLength = tilemap.localBounds.size.x;

        Debug.Log($"ParallaxGrid: {gameObject.name} | tileLength: {tileLength} | filhos: {children.Length}");
    }

    void Update()
    {
        if (!isMoving) return;

        foreach (var child in children)
        {
            child.Translate(Vector3.left * speed * Time.deltaTime);
        }

        // Checa se algum filho saiu da tela pela esquerda
        float camLeft = Camera.main.transform.position.x - (Camera.main.orthographicSize * Camera.main.aspect);

        foreach (var child in children)
        {
            // Borda direita desse filho
            float rightEdge = child.position.x + tileLength;

            if (rightEdge < camLeft)
            {
                // Acha o filho mais à direita
                float maxX = float.MinValue;
                foreach (var other in children)
                {
                    if (other != child && other.position.x > maxX)
                        maxX = other.position.x;
                }

                // Cola atrás do mais à direita
                child.position = new Vector3(maxX + tileLength, child.position.y, child.position.z);

                Debug.Log($"Reposicionou {child.name} pra X={child.position.x}");
            }
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